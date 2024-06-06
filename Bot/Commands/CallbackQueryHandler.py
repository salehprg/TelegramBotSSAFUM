from datetime import datetime, timedelta
from typing import Optional

from telethon import TelegramClient
from telethon.errors.rpcerrorlist import PeerIdInvalidError
from telethon.tl import functions, types
from telethon.tl.functions.messages import GetFullChatRequest
from telethon.tl.types import InputPeerChannel
from telethon.types import UpdateNewChannelMessage

from apihelper.BotSettingsServiceApi import BotSettingsServiceApi
from apihelper.PostServiceApi import PostServiceApi
from Commands.MessageStatus import MessagesStatus
from utility.inline_replies import *


class CallbackQueryHandler:
    def __init__(self, client: TelegramClient,user_client: TelegramClient,
                 bot_settings_service : BotSettingsServiceApi, 
                 post_service : PostServiceApi):
        
        self.user_client = user_client
        self.client = client
        self.bot_settings_service = bot_settings_service
        self.post_service = post_service
        
        self.bot_settings = self.bot_settings_service.botsettings

    async def update_forwarded_post_status(self, post_info_entity, status: str):
        post_events = self.post_service.get_removable_events(post_info_entity["id"])

        for post_event in post_events:
            try:
                await self.client.edit_message(
                    post_event["telegramId_ChatId"],
                    message=post_event["telegramId_PostId"],
                    text=status,
                )
            except Exception as ex:
                print(f"Error editing message: {ex}")
                continue
        
    async def on_set_bridge_channel(self, message, bridge_channel_id: str):
        self.bot_settings = self.bot_settings_service.get_bot_settings()
        
        chat_id = message.chat_id
        message_id = message.message_id

        new_status = "<b>این کانال به عنوان کانال میانی تنظیم شد ✅</b>"

        self.bot_settings["bridgeChanneID"] = int(bridge_channel_id)

        # Call the service to set bot settings
        self.bot_settings = self.bot_settings_service.set_bot_settings(self.bot_settings)

        try:
            # Edit the message text
            await self.client.edit_message(
                chat_id,
                message=message_id,
                text=new_status,
                parse_mode='html'
            )
        except Exception as e:
            print(f"Error editing message: {e}")
            
            
    async def on_accept_post(self, message):
        self.bot_settings = self.bot_settings_service.get_bot_settings()
        
        data = message.data.decode('utf-8').split("/")
        post_info_entity = self.post_service.get_post_by_id(int(data[1]))

        accepted_post = self.post_service.accept_post(post_info_entity)

        chat_id = message.chat_id
        message_id = message.message_id
        first_name = message.sender.first_name
        username = message.sender.username or ""

        new_status = MessagesStatus.accept_message(first_name , username)

        try:
            date_format = "%m/%d/%Y %H:%M:%S"
            date_time = datetime.strptime(accepted_post["forwardToChannelDate"], date_format)
        except:
            date_format = '%Y-%m-%dT%H:%M:%S.%fZ'
            date_time = datetime.strptime(accepted_post["forwardToChannelDate"], date_format)
            
        fwd_msg_events = await self.user_client(functions.messages.ForwardMessagesRequest(
            from_peer=post_info_entity["telegramId_ChatId"],
            id=post_info_entity['telegram_mediagroup_postids'],
            to_peer=self.bot_settings["mainChanneID"],
            schedule_date=date_time
        ))
        
        for updt in fwd_msg_events.updates:
            if type(updt) == UpdateNewChannelMessage and updt.message != '':
                sent_message = updt.message
                break

        # log_message = f"Forward to channel: {post_info_entity.telegram_id_post_id}, {post_info_entity.telegram_id_chat_id}, postid: {post_info_entity.id}"
        # logger.log_info(log_message)

        await self.client.edit_message(
            chat_id,
            message=message_id,
            text=new_status
        )

        await self.update_forwarded_post_status(post_info_entity, new_status)
        
    async def on_decline_post(self, message):
        self.bot_settings = self.bot_settings_service.get_bot_settings()
        
        data = message.data.decode('utf-8').split("/")
        post_info_entity = self.post_service.get_post_by_id(int(data[1]))

        chat_id = message.chat_id
        message_id = message.message_id
        first_name = message.sender.first_name
        username = message.sender.username or ""

        self.post_service.decline_post(post_info_entity)

        new_status = MessagesStatus.decline_message(first_name, username)

        await self.client.edit_message(
            chat_id,
            message=message_id,
            text=new_status
        )

        await self.update_forwarded_post_status(post_info_entity, new_status)
        
        
    async def on_inspect_more_post(self , message):
        self.bot_settings = self.bot_settings_service.get_bot_settings()
        
        data = message.data.decode('utf-8').split("/")
        post_info_entity = self.post_service.get_post_by_id(int(data[1]))

        chat_id = message.chat_id
        message_id = message.message_id
        first_name = message.sender.first_name
        username = message.sender.username or ""

        new_status = MessagesStatus.more_message(first_name, username)
        markup = get_inline_keyboard(post_info_entity["id"])

        # Forward the original message to the inspection group
        fwd_msg_events = await self.client(functions.messages.ForwardMessagesRequest(
            from_peer=post_info_entity["telegramId_ChatId"],
            id=post_info_entity['telegram_mediagroup_postids'],
            to_peer=self.bot_settings["inspectGroupID"],
            top_msg_id=self.bot_settings["inspectGroupTopicID"],
        ))
        
        for updt in fwd_msg_events.updates:
            if type(updt) == UpdateNewChannelMessage and updt.message != '':
                sent_message = updt.message
                break

        # Send a text message with the inspection status
        markup_message = await self.client.send_message(
            entity=self.bot_settings["inspectGroupID"],
            message="وضعیت پست:",
            reply_to=sent_message,
            buttons=get_inspect_more_inline_keyboard(post_info_entity["id"]),
        )

        # Update database with inspected post details
        post_info_telegram_model = {
            "telegramId_ChatId": markup_message.chat_id,
            "telegramId_PostId": markup_message.id,
            "repliyed_TelegramId_ChatId": sent_message.chat_id,
            "repliyed_TelegramId_PostId": sent_message.id,
        }
        self.post_service.inspect_post(post_info_entity, post_info_telegram_model)

        # Edit the original callback query message with new status and markup
        await self.client.edit_message(
            chat_id,
            message=message_id,
            text=new_status,
            buttons=markup,
        )
        
    async def on_license_check_post(self, message):
        self.bot_settings = self.bot_settings_service.get_bot_settings()
        
        data = message.data.decode('utf-8').split("/")
        post_info_entity = self.post_service.get_post_by_id(int(data[1]))

        chat_id = message.chat_id
        message_id = message.message_id
        first_name = message.sender.first_name
        username = message.sender.username or ""

        new_status = MessagesStatus.license_check_message(first_name, username)
        markup = get_inline_keyboard(post_info_entity["id"])

        try:
            
            fwd_msg_events = await self.client(functions.messages.ForwardMessagesRequest(
                from_peer=post_info_entity["telegramId_ChatId"],
                id=post_info_entity['telegram_mediagroup_postids'],
                to_peer=self.bot_settings["licenseGroupID"],
                top_msg_id=self.bot_settings["licenseGroupTopicID"],
            ))
            
            for updt in fwd_msg_events.updates:
                if type(updt) == UpdateNewChannelMessage and updt.message != '':
                    sent_message = updt.message
                    break
        
            # Send a text message with the license check status
            markup_message = await self.client.send_message(
                entity=self.bot_settings["licenseGroupID"],
                message="وضعیت پست:",
                reply_to=sent_message,
                buttons=get_license_check_inline_keyboard(post_info_entity["id"]),
            )

            # Update database with license check post details
            post_info_telegram_model = {
                "telegramId_ChatId": markup_message.chat_id,
                "telegramId_PostId": markup_message.id,
                "repliyed_TelegramId_ChatId": sent_message.chat_id,
                "repliyed_TelegramId_PostId": sent_message.id,
            }
            self.post_service.license_check_post(post_info_entity, post_info_telegram_model)

            # Edit the original callback query message with new status and markup
            await self.client.edit_message(
                chat_id,
                message=message_id,
                text=new_status,
                buttons=markup,
            )

        except Exception as e:
            print(f"An error occurred: {e}")
            
    async def on_set_inspect_topic_group(self , message):
        self.bot_settings = self.bot_settings_service.get_bot_settings()
        
        data = message.data.decode('utf-8').split("/")
        group_id = int(data[1])
        topic_id = int(data[2])

        chat_id = message.chat_id
        message_id = message.message_id

        new_status = "<b>این تاپیک به عنوان تاپیک بازرسی تنظیم شد ✅</b>"

        # Update bot settings
        self.bot_settings["inspectGroupID"] = group_id
        self.bot_settings["inspectGroupTopicID"] = topic_id

        # Save updated bot settings (replace with your actual service call)
        self.bot_settings = self.bot_settings_service.set_bot_settings(self.bot_settings)

        # log_message = f"Set Inspect Topic Group: {group_id} {topic_id}"
        # logger.log_info(log_message)

        # Edit the message with the new status
        await self.client.edit_message(
            chat_id,
            message=message_id,
            text=new_status,
            parse_mode="html"
        )


    async def on_set_license_check_group(self,message):
        self.bot_settings = self.bot_settings_service.get_bot_settings()
        
        data = message.data.decode('utf-8').split("/")
        group_id = int(data[1])
        topic_id = int(data[2])

        chat_id = message.chat_id
        message_id = message.message_id

        new_status = "<b>این تاپیک به عنوان تاپیک بررسی مجوز تنظیم شد ✅</b>"

        # Update bot settings
        self.bot_settings["licenseGroupID"] = group_id
        self.bot_settings["licenseGroupTopicID"] = topic_id

        # Save updated bot settings (replace with your actual service call)
        self.bot_settings = self.bot_settings_service.set_bot_settings(self.bot_settings)

        # log_message = f"Set License Check Topic Group: {group_id} {topic_id}"
        # logger.log_info(log_message)

        # Edit the message with the new status
        await self.client.edit_message(
            chat_id,
            message=message_id,
            text=new_status,
            parse_mode='html',
        )

    async def on_set_main_alternative_group_topic(self,message):
        self.bot_settings = self.bot_settings_service.get_bot_settings()
        
        data = message.data.decode('utf-8').split("/")
        group_id = int(data[1])
        topic_id = int(data[2])

        chat_id = message.chat_id
        message_id = message.message_id

        new_status = "<b>این تاپیک به عنوان تاپیک ارسال پست ها تنظیم شد ✅</b>"

        # Update bot settings
        self.bot_settings["alternativeMainGroupID"] = group_id
        self.bot_settings["alternativeMainGroupTopicID"] = topic_id

        # Save updated bot settings (replace with your actual service call)
        self.bot_settings = self.bot_settings_service.set_bot_settings(self.bot_settings)

        # log_message = f"Set Alternative Topic Group: {group_id} {topic_id}"
        # logger.log_info(log_message)

        # Edit the message with the new status
        await self.client.edit_message(
            chat_id,
            message=message_id,
            text=new_status,
            parse_mode='html',
        )