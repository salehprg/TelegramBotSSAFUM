using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Application.Interfaces;
using TelegramBot.Application.Services.BotSettings;
using TelegramBot.Application.Services.PostService;
using TelegramBot.Core.Entities;

namespace TelegramBot.Bot
{
    public class CallbackQueryHandler
    {
        IMapper mapper;
        BotSettingsEntity botSettings;
        IBotSettingsService botSettingsService;
        ILoggerService logger;
        IPostService postService;
        private readonly ITelegramBotClient botClient;
        private readonly Update update;
        private readonly CancellationToken cancellationToken;

        public CallbackQueryHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, BotSettingsEntity botSettings, ILoggerService logger, IMapper mapper, IBotSettingsService botSettingsService, IPostService postService)
        {
            this.botClient = botClient;
            this.update = update;
            this.cancellationToken = cancellationToken;
            this.botSettings = botSettings;
            this.logger = logger;
            this.mapper = mapper;
            this.botSettingsService = botSettingsService;
            this.postService = postService;
        }

        async void UpdateForwardedPostStatus(PostInfoEntity postInfoEntity, string status, InlineKeyboardMarkup? markup)
        {

            var postEvents = postService.GetRemovableEvents(postInfoEntity.Id).ToList();
            foreach (var postEvent in postEvents)
            {
                try
                {
                    await botClient.EditMessageTextAsync(chatId: postEvent.TelegramId_ChatId,
                                            messageId: postEvent.TelegramId_PostId,
                                            text: status,
                                            replyMarkup: markup,
                                            cancellationToken: cancellationToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;
                }
            }
        }

        public async void OnSetBridgeChannel(CallbackQuery message, string bridgeChanneID)
        {
            var chatId = message.Message.Chat.Id;
            var messageId = message.Message.MessageId;

            var newStatus = "<b>این کانال به عنوان کانال میانی تنظیم شد ✅</b>";

            var newbotSettings = mapper.Map<BotSettingsModel>(botSettings);
            newbotSettings.BridgeChanneID = long.Parse(bridgeChanneID);

            botSettingsService.SetBotSettings(newbotSettings);

            var logMessage = $"Set Bridge channel: {bridgeChanneID}";
            logger.LogInfo(logMessage);

            await botClient.EditMessageTextAsync(chatId: chatId,
                                                    messageId: messageId,
                                                    text: newStatus,
                                                    parseMode: ParseMode.Html,
                                                    cancellationToken: cancellationToken);
        }

        public async void OnAcceptPost(CallbackQuery message)
        {
            var data = message.Data.Split("/");
            PostInfoEntity postInfoEntity = postService.GetByPostId(int.Parse(data[1]));

            var acceptedPost = postService.AcceptPost(postInfoEntity);

            var chatId = message.Message.Chat.Id;
            var messageId = message.Message.MessageId;
            var FirstName = message.From.FirstName;
            var Username = message.From.Username ?? "";

            var newStatus = MessagesStatus.AcceptMessage(FirstName, Username);

            // Message sentMessage = await botClient.ForwardMessageAsync(
            //             chatId: botSettings.BridgeChanneID,
            //             fromChatId: message.Message.ReplyToMessage.Chat.Id,
            //             messageId: message.Message.ReplyToMessage.MessageId,
            //             cancellationToken: cancellationToken);

            Message acceptMessage = await botClient.SendTextMessageAsync(
                chatId: botSettings.BridgeChanneID,
                text: $"{botSettings.MainChanneID},{acceptedPost.ForwardToChannelDate},{postInfoEntity.TelegramId_PostId},{postInfoEntity.TelegramId_ChatId}",
                cancellationToken: cancellationToken);

            var logMessage = $"Forward to channel: {postInfoEntity.TelegramId_PostId} , {postInfoEntity.TelegramId_ChatId} , postid: {postInfoEntity.Id}";
            logger.LogInfo(logMessage);

            await botClient.EditMessageTextAsync(chatId: chatId,
                                                        messageId: messageId,
                                                        text: newStatus,
                                                        cancellationToken: cancellationToken);

            UpdateForwardedPostStatus(postInfoEntity, newStatus, null);

        }

        public async void OnDeclinePost(CallbackQuery message)
        {
            var data = message.Data.Split("/");
            PostInfoEntity postInfoEntity = postService.GetByPostId(int.Parse(data[1]));

            var chatId = message.Message.Chat.Id;
            var messageId = message.Message.MessageId;
            var FirstName = message.From.FirstName;
            var Username = message.From.Username ?? "";

            postService.DeclinePost(postInfoEntity);

            var newStatus = MessagesStatus.DeclineMessage(FirstName, Username);

            await botClient.EditMessageTextAsync(chatId: chatId,
                                                        messageId: messageId,
                                                        text: newStatus,
                                                        cancellationToken: cancellationToken);

            UpdateForwardedPostStatus(postInfoEntity, newStatus, null);

        }

        public async void OnInspectMorePost(CallbackQuery message)
        {
            var data = message.Data.Split("/");
            PostInfoEntity postInfoEntity = postService.GetByPostId(int.Parse(data[1]));

            var chatId = message.Message.Chat.Id;
            var messageId = message.Message.MessageId;
            var FirstName = message.From.FirstName;
            var Username = message.From.Username ?? "";


            var newStatus = MessagesStatus.MoreMessage(FirstName, Username);
            var markup = Utility.getInlineKeyboard(postInfoEntity.Id);

            Message sentMessage = await botClient.ForwardMessageAsync(
                        chatId: botSettings.InspectGroupID,
                        messageThreadId: botSettings.InspectGroupTopicID,
                        fromChatId: message.Message.ReplyToMessage.Chat.Id,
                        messageId: message.Message.ReplyToMessage.MessageId,
                        cancellationToken: cancellationToken);

            var inlineKeyboard = Utility.getInspectMoreInlineKeyboard(postInfoEntity.Id);

            Message markupMessage = await botClient.SendTextMessageAsync(
                    chatId: botSettings.InspectGroupID,
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

            postService.InspectPost(postInfoEntity, postInfoTelegramModel);

            await botClient.EditMessageTextAsync(chatId: chatId,
                                            messageId: messageId,
                                            text: newStatus,
                                            replyMarkup: markup,
                                            cancellationToken: cancellationToken);

        }

        public async void OnLicenseCheckPost(CallbackQuery message)
        {
            var data = message.Data.Split("/");
            PostInfoEntity postInfoEntity = postService.GetByPostId(int.Parse(data[1]));

            var chatId = message.Message.Chat.Id;
            var messageId = message.Message.MessageId;
            var FirstName = message.From.FirstName;
            var Username = message.From.Username ?? "";

            var newStatus = MessagesStatus.LicenseCheckMessage(FirstName, Username);
            var markup = Utility.getInlineKeyboard(postInfoEntity.Id);

            Message sentMessage = await botClient.ForwardMessageAsync(
                        chatId: botSettings.LicenseGroupID,
                        messageThreadId: botSettings.LicenseGroupTopicID,
                        fromChatId: message.Message.ReplyToMessage.Chat.Id,
                        messageId: message.Message.ReplyToMessage.MessageId,
                        cancellationToken: cancellationToken);

            var inlineKeyboard = Utility.getLicenseCheckInlineKeyboard(postInfoEntity.Id);

            Message markupMessage = await botClient.SendTextMessageAsync(
                    chatId: botSettings.LicenseGroupID,
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

            postService.LicenseCheckPost(postInfoEntity, postInfoTelegramModel);

            try
            {
                await botClient.EditMessageTextAsync(chatId: chatId,
                                                messageId: messageId,
                                                text: newStatus,
                                                replyMarkup: markup,
                                                cancellationToken: cancellationToken);
            }
            catch (System.Exception)
            {

                return;
            }

        }

        public async void OnSetInspectTopicGroup(CallbackQuery message)
        {
            var data = message.Data.Split("/");
            var groupId = long.Parse(data[1]);
            var topicId = int.Parse(data[2]);

            var chatId = message.Message.Chat.Id;
            var messageId = message.Message.MessageId;

            var newStatus = "<b>این تاپیک به عنوان تاپیک بازرسی تنظیم شد ✅</b>";

            var newbotSettings = mapper.Map<BotSettingsModel>(botSettings);
            newbotSettings.InspectGroupID = groupId;
            newbotSettings.InspectGroupTopicID = topicId;

            botSettingsService.SetBotSettings(newbotSettings);

            var logMessage = $"Set Inspect Topic Group: {groupId} {topicId}";
            logger.LogInfo(logMessage);

            await botClient.EditMessageTextAsync(chatId: chatId,
                                                    messageId: messageId,
                                                    text: newStatus,
                                                    parseMode: ParseMode.Html,
                                                    cancellationToken: cancellationToken);
        }

        public async void OnSetLicenseCheckGroup(CallbackQuery message)
        {
            var data = message.Data.Split("/");
            var groupId = long.Parse(data[1]);
            var topicId = int.Parse(data[2]);

            var chatId = message.Message.Chat.Id;
            var messageId = message.Message.MessageId;

            var newStatus = "<b>این تاپیک به عنوان تاپیک بررسی مجوز تنظیم شد ✅</b>";

            var newbotSettings = mapper.Map<BotSettingsModel>(botSettings);
            newbotSettings.LicenseGroupID = groupId;
            newbotSettings.LicenseGroupTopicID = topicId;

            botSettingsService.SetBotSettings(newbotSettings);

            var logMessage = $"Set License Check Topic Group: {groupId} {topicId}";
            logger.LogInfo(logMessage);

            await botClient.EditMessageTextAsync(chatId: chatId,
                                                    messageId: messageId,
                                                    text: newStatus,
                                                    parseMode: ParseMode.Html,
                                                    cancellationToken: cancellationToken);
        }

        public async void OnSetMainAlternativeGroupTopic(CallbackQuery message)
        {
            var data = message.Data.Split("/");
            var groupId = long.Parse(data[1]);
            var topicId = int.Parse(data[2]);

            var chatId = message.Message.Chat.Id;
            var messageId = message.Message.MessageId;

            var newStatus = "<b>این تاپیک به عنوان تاپیک ارسال پست ها تنظیم شد ✅</b>";

            var newbotSettings = mapper.Map<BotSettingsModel>(botSettings);
            newbotSettings.AlternativeMainGroupID = groupId;
            newbotSettings.AlternativeMainGroupTopicID = topicId;

            botSettingsService.SetBotSettings(newbotSettings);

            var logMessage = $"Set Alternative Topic Group: {groupId} {topicId}";
            logger.LogInfo(logMessage);

            await botClient.EditMessageTextAsync(chatId: chatId,
                                                    messageId: messageId,
                                                    text: newStatus,
                                                    parseMode: ParseMode.Html,
                                                    cancellationToken: cancellationToken);
        }

    }
}