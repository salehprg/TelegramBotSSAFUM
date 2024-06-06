
from telethon import TelegramClient, functions
from telethon.types import UpdateNewChannelMessage

from apihelper.BotSettingsServiceApi import BotSettingsServiceApi
from apihelper.PostServiceApi import PostServiceApi
from Commands.MessageStatus import MessagesStatus
from utility.inline_replies import get_inline_keyboard


class ChannelPostHandler:
    def __init__(self, client: TelegramClient, 
                 bot_settings_service : BotSettingsServiceApi, 
                 post_service : PostServiceApi):
        
        self.bot_client = client
        self.bot_settings_service = bot_settings_service
        self.post_service = post_service
    
    def check_mention_channel_tag(self, text, channel_id):
        if f"@{channel_id.lower()}" in text.lower():
            return True
        return False

    async def on_channel_post(self , post, ignore_list):
        
        botsetting = self.bot_settings_service.get_bot_settings()
        
        org_message_ids = [x.forward.channel_post for x in post]
        
        brdg_message_ids = [x.id for x in post]

        mainpost = post[0]
        
        post_info_entity = self.post_service.create_new_post({
            "TelegramId_PostId":mainpost.forward.channel_post,
            "TelegramId_ChatId":mainpost.forward.chat_id,
            "Telegram_mediagroup_postids":org_message_ids,
            "Media_groupId":0,
        })

        if mainpost.forward.chat_id in ignore_list:
            return

        inline_keyboard = get_inline_keyboard(post_info_entity["id"])

        mentioned = self.check_mention_channel_tag(mainpost.message, botsetting["mainChannelUsername"])
        print(f"Channel Mentioned: {mentioned}")

        if mentioned:
            try:
                
                chatId = botsetting["alternativeMainGroupID"] if botsetting["alternativeMainGroupID"] != 0 else botsetting["mainGroupID"]
                
                fwd_msg_events = await self.bot_client(functions.messages.ForwardMessagesRequest(
                    from_peer=mainpost.chat_id,
                    id=brdg_message_ids,
                    to_peer=chatId,
                    top_msg_id=botsetting["alternativeMainGroupTopicID"],
                ))
                
                for updt in fwd_msg_events.updates:
                    if type(updt) == UpdateNewChannelMessage and updt.message != '':
                        sent_message = updt.message
                        break
            

                # log_message = f"Forward to group: {message_id}, {chat_id}, postid: {post_info_entity.id}"
                # logger.log_info(log_message)

                markup_message = await self.bot_client.send_message(
                    entity=sent_message.chat_id,
                    message=MessagesStatus.pending_message(mainpost.forward.date),
                    reply_to=sent_message.id,
                    buttons=inline_keyboard
                )

                post_info_telegram_model = {
                    "TelegramId_ChatId":markup_message.chat_id,
                    "TelegramId_PostId":markup_message.id,
                    "Repliyed_TelegramId_ChatId":sent_message.chat_id,
                    "Repliyed_TelegramId_PostId":sent_message.id
                }

                self.post_service.forward_to_group_post(post_info_entity, post_info_telegram_model)

                old_post_infos = self.post_service.get_events_by_telegram_id(post_info_entity["telegramId_ChatId"], post_info_entity["telegramId_PostId"])
                for old_data in old_post_infos:
                    if old_data["id"] != post_info_entity["id"]:
                        try:
                            post_events = self.post_service.get_removable_events(old_data["id"])
                            for post_event in post_events:
                                await self.bot_client.delete_messages(post_event["telegramId_ChatId"], [post_event["telegramId_PostId"], post_event["repliyed_TelegramId_PostId"]])
                            self.post_service.remove_old_post_status(old_data)
                        except Exception as ex:
                            print(ex)
                            return
            except Exception as ex:
                print(ex)
                return