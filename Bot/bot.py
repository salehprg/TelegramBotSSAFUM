import asyncio

from telethon import Button, TelegramClient, events, functions, types

from Commands.ChannelPostHandler import ChannelPostHandler
from apihelper.BotSettingsServiceApi import BotSettingsServiceApi
from apihelper.ChannelServiceApi import ChannelServiceApi
from apihelper.PostServiceApi import PostServiceApi
from Commands.CallbackQueryHandler import CallbackQueryHandler
from Commands.CommandHandler import CommandHandler
from utility.inline_replies import get_inline_keyboard, set_bridgeChannel_inline_keyboard


class Bot:
    def __init__(self , baseURL,api_id,api_hash,bot_token,user_client ) -> None:
        self.api_id = api_id
        self.api_hash = api_hash
        self.bot_token = bot_token
        self.client = TelegramClient('./session_bot', self.api_id, self.api_hash)
        
        self.bot_settings = BotSettingsServiceApi(baseURL)
        self.post_service = PostServiceApi(baseURL)
        self.channel_service = ChannelServiceApi(baseURL)
        
        self.callback_query_handler = CallbackQueryHandler(self.client,user_client, self.bot_settings,self.post_service)
        self.command_handler = CommandHandler(self.client, self.bot_settings)
        self.channel_post_handler = ChannelPostHandler(self.client , self.bot_settings , self.post_service)
        
        self.bridge_channelId = self.bot_settings.botsettings["bridgeChanneID"]
        self.post_group = self.bot_settings.botsettings["mainGroupID"] 
        
        @self.client.on(events.ChatAction)
        async def chat_action(event):
            
            if event.is_channel and not event.is_group and event.user_added and event.user.username == 'ssa_admin_post_bot':
                chat_id =event.chat_id
                
                inline_keyboard = set_bridgeChannel_inline_keyboard(chat_id)
                
                await self.client.send_message(
                    chat_id,
                    message="برای انتخاب این کانال به عنوان کانال میانی تایید کنید",
                    buttons=inline_keyboard
                )
                
            return
        
        @self.client.on(events.CallbackQuery)
        async def post_callback(event):
            message_data = event.data.decode('utf-8')
            
            print(f"Received a '{message_data}' message in chat")

            data = message_data.split("/")

            if data[0] == "bridgeChannel":
                await self.callback_query_handler.on_set_bridge_channel(event, data[1])
            else:
                if data[0] == "accept":
                    await self.callback_query_handler.on_accept_post(event)
                elif data[0] == "license_check":
                    await self.callback_query_handler.on_license_check_post(event)
                elif data[0] == "set_Main_Alternative_Group_Topic":
                    await self.callback_query_handler.on_set_main_alternative_group_topic(event)
                elif data[0] == "set_license_check_topic":
                    await self.callback_query_handler.on_set_license_check_group(event)
                elif data[0] == "inspect_topic":
                    await self.callback_query_handler.on_set_inspect_topic_group(event)
                elif data[0] == "decline":
                    await self.callback_query_handler.on_decline_post(event)
                elif data[0] == "more":
                    await self.callback_query_handler.on_inspect_more_post(event)
            
            return

        @self.client.on(events.NewMessage)
        async def handle_command_message(event):
            message = event.message
            message_text = message.message

            if message_text.startswith("/set_inspect_topic"):
                await self.command_handler.on_set_inspect_topic(message)
                return

            if message_text.startswith("/set_license_check_topic"):
                await self.command_handler.on_set_license_check_topic(message)
                return

            if message_text.startswith("/set_forward_post_topic"):
                await self.command_handler.on_set_main_alternative_group_topic(message)
                return

            if message_text.startswith("/set_maingroup"):
                await self.command_handler.on_set_main_group(message)
                return

            if message_text.startswith("/set_mainchannel"):
                await self.command_handler.on_set_main_channel(message)
                return

            # if message_text.startswith("/channel_list"):
            #     results = channel_service.get_channel_list()

            #     text = ""
            #     for channel in results:
            #         activestatus = "✅" if channel.is_active else "❌"
            #         text += f"{channel.name} - @{channel.username}  {activestatus}\n"

            #     await bot_client.send_message(
            #         chat_id=chat_id,
            #         message=text
            #     )
            #     return

    async def forward_media_group(self, fwd_msg):
                    
        channel_list = self.channel_service.get_deactivated_channels()
        
        ignore_list = [channel["channelTelegramId"] for channel in channel_list]
        
        await self.channel_post_handler.on_channel_post(fwd_msg,ignore_list)
        
        return
    
        
    async def run_bot(self):
        await self.client.start(bot_token=self.bot_token)
        print("Bot Started!")

        # Keep the client running
        await self.client.run_until_disconnected()