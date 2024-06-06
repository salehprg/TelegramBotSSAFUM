using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Application.Interfaces;
using TelegramBot.Application.Services.BotSettings;
using TelegramBot.Application.Services.PostService;
using TelegramBot.Core.Entities;

namespace TelegramBot.Bot
{
    public class CommandHandler
    {
        IMapper mapper;
        BotSettingsEntity botSettings;
        IBotSettingsService botSettingsService;
        ILoggerService logger;
        private readonly ITelegramBotClient botClient;
        private readonly Update update;
        private readonly CancellationToken cancellationToken;

        public CommandHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, BotSettingsEntity botSettings, ILoggerService logger, IMapper mapper, IBotSettingsService botSettingsService)
        {
            this.botClient = botClient;
            this.update = update;
            this.cancellationToken = cancellationToken;
            this.botSettings = botSettings;
            this.logger = logger;
            this.mapper = mapper;
            this.botSettingsService = botSettingsService;
        }


        public async void OnSetMainGroup(Message message)
        {
            var chatId = message.Chat.Id;

            var newbotSettings = mapper.Map<BotSettingsModel>(botSettings);

            newbotSettings.MainGroupID = chatId;
            newbotSettings.MainGroupUsername = update.Message.Chat.Title;

            botSettingsService.SetBotSettings(newbotSettings);

            var logMessage = $"Set Main Group: {update.Message.Chat.Title} , {chatId}";
            logger.LogInfo(logMessage);

            Message setMainChannel = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "این گروه به عنوان گروه اصلی ثبت شد",
                cancellationToken: cancellationToken);
        }

        public async void OnSetMainChannel(Message message)
        {
            var messageText = message.Text;
            var chatId = message.Chat.Id;

            var channelId = messageText.Split("\n");

            Chat mainChannel = await botClient.GetChatAsync($"{channelId[1]}");

            var newbotSettings = mapper.Map<BotSettingsModel>(botSettings);
            newbotSettings.MainChanneID = mainChannel.Id;
            newbotSettings.MainChannelUsername = mainChannel.Username;


            botSettingsService.SetBotSettings(newbotSettings);

            var logMessage = $"Set Main Channel: {mainChannel.Username} , {mainChannel.Id}";
            logger.LogInfo(logMessage);

            Message setMainChannel = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: $" کانال {channelId[1]} به عنوان کانال اصلی ثبت شد ",
                cancellationToken: cancellationToken);
        }

        public async void OnSetInspectTopic(Message message)
        {
            if (message.MessageThreadId is { } topicId)
            {
                var chatId = message.Chat.Id;

                var inlineKeyboard = Utility.setInspectTopic_InlineKeyboard(chatId, topicId);

                Message markupMessage = await botClient.SendTextMessageAsync(
                                            chatId: chatId,
                                            text: "برای انتخاب این تاپیک به عنوان تاپیک بازرسی تایید کنید",
                                            replyMarkup: inlineKeyboard,
                                            replyToMessageId: topicId,
                                            cancellationToken: cancellationToken);
            }
        }

        public async void OnSetLicenseCheckTopic(Message message)
        {
            if (message.MessageThreadId is { } topicId)
            {
                var chatId = message.Chat.Id;

                var inlineKeyboard = Utility.setLicenseCheckTopic_InlineKeyboard(chatId, topicId);

                Message markupMessage = await botClient.SendTextMessageAsync(
                                            chatId: chatId,
                                            text: "برای انتخاب این تاپیک به عنوان تاپیک بررسی مجوز تایید کنید",
                                            replyMarkup: inlineKeyboard,
                                            replyToMessageId: topicId,
                                            cancellationToken: cancellationToken);
            }
        }

        public async void OnSetMainAlternativeGroupTopic(Message message)
        {
            if (message.MessageThreadId is { } topicId)
            {
                var chatId = message.Chat.Id;

                var inlineKeyboard = Utility.setMainAlternativeGroupTopic_InlineKeyboard(chatId, topicId);

                Message markupMessage = await botClient.SendTextMessageAsync(
                                            chatId: chatId,
                                            text: "برای انتخاب این تاپیک به عنوان تاپیک ارسال پست ها تایید کنید",
                                            replyMarkup: inlineKeyboard,
                                            replyToMessageId: topicId,
                                            cancellationToken: cancellationToken);
            }
        }

        public void OnEditedChannelPost()
        {

        }
    }
}