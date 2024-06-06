from telethon import TelegramClient
from telethon.tl.types import Message

from apihelper.BotSettingsServiceApi import BotSettingsServiceApi
from Models.BotSettingsModel import BotSettingsModel
from utility.inline_replies import *


class CommandHandler:
    def __init__(self, bot_client: TelegramClient , bot_settings_service : BotSettingsServiceApi):
        self.bot_client = bot_client
        self.bot_settings_service = bot_settings_service
        
        self.bot_settings = self.bot_settings_service.botsettings

    async def on_set_main_group(self, message: Message):
        chat_id = message.chat_id

        self.bot_settings = self.bot_settings_service.get_bot_settings()
        
        self.bot_settings["mainGroupID"] = chat_id
        self.bot_settings["mainGroupUsername"] = message.chat.title

        self.bot_settings = self.bot_settings_service.set_bot_settings(self.bot_settings)

        # log_message = f"Set Main Group: {message.chat.title}, {chat_id}"
        # self.logger.log_info(log_message)

        await self.bot_client.send_message(
            chat_id,
            message="این گروه به عنوان گروه اصلی ثبت شد"
        )

    async def on_set_main_channel(self, message: Message):
        message_text = message.text
        chat_id = message.chat_id

        channel_id = message_text.split("\n")[1]
        main_channel = await self.bot_client.get_entity(channel_id)

        self.bot_settings = self.bot_settings_service.get_bot_settings()
        
        self.bot_settings["mainChanneID"] = int(f"-100{main_channel.id}")
        self.bot_settings["mainChannelUsername"] = main_channel.username

        self.bot_settings = self.bot_settings_service.set_bot_settings(self.bot_settings)

        await self.bot_client.send_message(
            chat_id,
            message=f"کانال {channel_id} به عنوان کانال اصلی ثبت شد"
        )

    async def on_set_inspect_topic(self, message: Message):
        topic_id = message.reply_to_msg_id
        if topic_id:
            chat_id = message.chat_id

            inline_keyboard = set_inspect_topic_inline_keyboard(chat_id, topic_id)

            await self.bot_client.send_message(
                chat_id,
                message="برای انتخاب این تاپیک به عنوان تاپیک بازرسی تایید کنید",
                buttons=inline_keyboard,
                reply_to=topic_id
            )

    async def on_set_license_check_topic(self, message: Message):
        topic_id = message.reply_to_msg_id
        if topic_id:
            chat_id = message.chat_id

            inline_keyboard = set_license_check_topic_inline_keyboard(chat_id, topic_id)

            await self.bot_client.send_message(
                chat_id,
                message="برای انتخاب این تاپیک به عنوان تاپیک بررسی مجوز تایید کنید",
                buttons=inline_keyboard,
                reply_to=topic_id
            )

    async def on_set_main_alternative_group_topic(self, message: Message):
        topic_id = message.reply_to_msg_id
        if topic_id:
            chat_id = message.chat_id

            inline_keyboard = set_main_alternative_group_topic_inline_keyboard(chat_id, topic_id)

            await self.bot_client.send_message(
                chat_id,
                message="برای انتخاب این تاپیک به عنوان تاپیک ارسال پست ها تایید کنید",
                buttons=inline_keyboard,
                reply_to=topic_id
            )
