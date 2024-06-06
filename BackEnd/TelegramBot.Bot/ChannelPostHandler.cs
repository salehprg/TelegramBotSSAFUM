using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Application.Interfaces;
using TelegramBot.Application.Services.PostService;
using TelegramBot.Core.Entities;

namespace TelegramBot.Bot
{
    public class ChannelPostHandler
    {
        IPostService postService;
        BotSettingsEntity botSettings;
        ILoggerService logger;
        private readonly ITelegramBotClient botClient;
        private readonly Update update;
        private readonly CancellationToken cancellationToken;

        public ChannelPostHandler(IPostService postService,
                                    ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, BotSettingsEntity botSettings, ILoggerService logger)
        {
            this.postService = postService;
            this.botClient = botClient;
            this.update = update;
            this.cancellationToken = cancellationToken;
            this.botSettings = botSettings;
            this.logger = logger;
        }

        bool CheckMentionChannelTag(List<string> Entities, string channelId)
        {
            foreach (var entity in Entities)
            {
                if (entity.Equals($"@{channelId.ToLower()}", StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public async void OnChannelPost(Message post, List<long> ignoreList)
        {
            var chatId = post.Chat.Id;
            var messageId = post.MessageId;

            PostInfoEntity postInfoEntity = postService.SaveNewPost(new PostInfoEntity()
            {
                TelegramId_PostId = (int)post.ForwardFromMessageId,
                TelegramId_ChatId = post.ForwardFromChat.Id,
            });

            List<string> entities = new();

            if (post.ForwardFromChat is { } forwardedChat)
            {
                if (ignoreList.Contains(forwardedChat.Id))
                {
                    return;
                }
            }

            if (post.CaptionEntityValues != null)
            {
                entities = post.CaptionEntityValues.ToList();
            }
            else if (post.EntityValues != null)
            {
                entities = post.EntityValues.ToList();
            }

            var inlineKeyboard = Utility.getInlineKeyboard(postInfoEntity.Id);

            var mentioned = CheckMentionChannelTag(entities, botSettings.MainChannelUsername);
            Console.WriteLine($"Channel Mentioned : {mentioned}");

            if (mentioned)
            {
                Message sentMessage = new Message();

                if (botSettings.AlternativeMainGroupID != 0)
                {
                    sentMessage = await botClient.ForwardMessageAsync(
                    chatId: botSettings.AlternativeMainGroupID,
                    messageThreadId: botSettings.AlternativeMainGroupTopicID,
                    fromChatId: chatId,
                    messageId: messageId,
                    cancellationToken: cancellationToken);
                }
                else
                {
                    sentMessage = await botClient.ForwardMessageAsync(
                        chatId: botSettings.MainGroupID,
                        fromChatId: chatId,
                        messageId: messageId,
                        cancellationToken: cancellationToken);
                }

                var logMessage = $"Forward to group: {messageId} , {chatId} , postid: {postInfoEntity.Id}";
                logger.LogInfo(logMessage);

                Message markupMessage = await botClient.SendTextMessageAsync(
                    chatId: sentMessage.Chat.Id,
                    text: "وضعیت پست:",
                    replyMarkup: inlineKeyboard,
                    replyToMessageId: sentMessage.MessageId,
                    cancellationToken: cancellationToken);

                PostInfoTelegramModel postInfoTelegramModel = new()
                {
                    TelegramId_ChatId = markupMessage.Chat.Id,
                    TelegramId_PostId = markupMessage.MessageId,
                    Repliyed_TelegramId_ChatId = sentMessage.Chat.Id,
                    Repliyed_TelegramId_PostId = sentMessage.MessageId
                };

                postService.ForwardToGroupPost(postInfoEntity, postInfoTelegramModel);

                var oldPostInfos = postService.GetEventsByTelegramId(postInfoEntity.TelegramId_ChatId, postInfoEntity.TelegramId_PostId).ToList();
                foreach (var oldDatas in oldPostInfos)
                {
                    if (oldDatas.Id != postInfoEntity.Id)
                        try
                        {
                            var postEvents = postService.GetRemovableEvents(oldDatas.Id);
                            foreach (var postEvent in postEvents)
                            {
                                await botClient.DeleteMessageAsync(postEvent.TelegramId_ChatId, postEvent.TelegramId_PostId, cancellationToken);
                                await botClient.DeleteMessageAsync(postEvent.Repliyed_TelegramId_ChatId, postEvent.Repliyed_TelegramId_PostId, cancellationToken);
                            }
                            postService.RemoveOldPostStatus(oldDatas);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            return;
                        }
                }
            }
        }

        public void OnEditedChannelPost()
        {

        }
    }
}