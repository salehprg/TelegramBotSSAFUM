import asyncio
import os
import threading
from datetime import datetime, timedelta
from typing import List

import aiojobs
from telethon import TelegramClient, events, functions
from telethon.types import UpdateNewChannelMessage

from apihelper.ChannelServiceApi import ChannelServiceApi
from bot import Bot
from Models.ChannelInfoModel import ChannelInfoModel

api_id = os.environ.get("SSAFUM_apiID" , "")
api_hash = os.environ.get("SSAFUM_apiHASH", "")

client = TelegramClient('./session_main', api_id, api_hash)
channelService = ChannelServiceApi(os.environ.get("Backend_Host" , "http://localhost:5293"))

bot_client = Bot(baseURL=os.environ.get("Backend_Host" , "http://localhost:5293"),
                 api_id=os.environ.get("SSAFUM_apiID" , ""),
                 api_hash=os.environ.get("SSAFUM_apiHASH", ""),
                 bot_token=os.environ.get("SSAFUM_botToken", ""),
                 user_client=client)

timers = {}
message_ids = {}
lock = asyncio.Lock()

@client.on(events.MessageEdited)
async def new_edit_post(event):
    await post_analyser(event)
    
@client.on(events.NewMessage)
async def new_post(event):
    await post_analyser(event)

async def forward_to_bridge(from_postId, from_chatId):

    fwd_msg = await client.forward_messages(bot_client.bot_settings.get_bot_settings()["bridgeChanneID"] , from_postId , from_chatId)
        
    await bot_client.forward_media_group(fwd_msg)


async def post_analyser(event):

    botsetting = bot_client.bot_settings.get_bot_settings()
    
    try:
        if not event.is_group and event.message.chat_id != botsetting["mainChanneID"] and event.message.chat_id != botsetting["bridgeChanneID"]:
            message = event.message
            
            if message.fwd_from != None:
                return
            
            if event.message.grouped_id is not None:
                # Start or reset timer for the media group ID
                global timers, message_ids
                async with lock:
                    
                    groupid = message.grouped_id
                    if groupid not in timers:
                        # Create a new timer for this media group ID
                        timers[groupid] = asyncio.create_task(start_timer(groupid, event.message.chat_id))

                        # Add message ID to the list of message IDs for this media group ID
                        message_ids[groupid] = []

                    message_ids[groupid].append(message.id)

                    timers[groupid].cancel()
                    timers[groupid] = asyncio.create_task(start_timer(groupid, event.message.chat_id))
            
            else:
                await forward_to_bridge([message.id] ,message.chat_id)
            # elif event.message.chat_id == -1002033654942:
            #     forwardChatId, time , messageid , chatid = event.message.raw_text.split(",")
                
            #     date_format = "%m/%d/%Y %H:%M:%S"
            #     date_time = datetime.strptime(time, date_format)
                
            #     await client.forward_messages(int(forwardChatId) , int(messageid) , int(chatid), schedule=date_time)
    except Exception as ex:
        print(ex)
        return
    
async def start_timer(groupid, chatid):
    try:
        await asyncio.sleep(5)  # Wait for 5 seconds
        await timer_callback(groupid, chatid)
    except asyncio.CancelledError:
        pass  # Timer was cancelled, restart the timer if necessary

async def timer_callback(media_group_id, chatid):
    global timers, message_ids
    for message_id in message_ids[media_group_id]:
        print(message_id)

    try:
        async with lock:
            await forward_to_bridge(message_ids[media_group_id] ,chatid)
    except Exception as ex:
        print(ex)
        return
    finally:
        async with lock:
            del timers[media_group_id]
            del message_ids[media_group_id]
        

async def getChannels():

    dialogs = client.iter_dialogs()
    subscribed_channels : List[ChannelInfoModel] = []

    async for dialog in dialogs:
        if dialog.is_channel and not dialog.is_group:
            channel_info = ChannelInfoModel(
                name=dialog.entity.title,
                username=dialog.entity.username,
                description=None,  # Assuming description is not available
                phone=None,  # Assuming phone is not available
                channelTelegramId=dialog.id
            )
            subscribed_channels.append(channel_info)

    print("Subscribed Channels:")
    for channel_info in subscribed_channels:
        print(f"Channel ID: {channel_info.channelTelegramId}, Title: {channel_info.name}")
    
    try:
        response = channelService.update_multiple([x.to_dict() for x in subscribed_channels])
        print("Sync Response:", response)
        
    except Exception as ex:
        print("Sync Error " , datetime.now())
        print(ex)
        
        return
     

async def sync_channels_task():
   await getChannels()
    
async def scheduler():
    scheduler = await aiojobs.create_scheduler()
    while True:
        now = datetime.now()
        next_run = now.replace(hour=0, minute=0, second=0, microsecond=0)
        if now >= next_run:
            next_run += timedelta(days=1)
        delay = (next_run - now).total_seconds()
        await asyncio.sleep(delay)
        await sync_channels_task()
    
print("Client Started!")

loop = asyncio.get_event_loop()

client.start(phone=os.environ.get("SSAFUM_phone" , ""))
loop.create_task(bot_client.run_bot(), name="Bot")

loop.create_task(scheduler() , name="Schedule")

client.loop.run_until_complete(getChannels())
client.run_until_disconnected()
