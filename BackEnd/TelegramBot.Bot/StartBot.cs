using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Application.Interfaces;
using TelegramBot.Application.Services.BotSettings;
using TelegramBot.Application.Services.ChannelService;
using TelegramBot.Application.Services.PostService;
using TelegramBot.Core.Entities;

namespace TelegramBot.Bot;

public class StartBot
{
    IMapper mapper;
    ILoggerService logger;
    IPostService postService;
    IChannelService channelService;
    IBotSettingsService botSettingsService;

    public static TelegramBotClient botClient;
    public StartBot(IMapper mapper, IPostService postService, IBotSettingsService botSettingsService, ILoggerService logger, IChannelService channelService)
    {
        this.mapper = mapper;
        this.postService = postService;
        this.botSettingsService = botSettingsService;
        this.logger = logger;
        this.channelService = channelService;
    }

    public async void Start()
    {
        BotSettingsEntity botSettings = botSettingsService.GetBotSettings();

        botClient = new TelegramBotClient(botSettings.botToken);

        using CancellationTokenSource cts = new();

        // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
        };

        botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

        var me = await botClient.GetMeAsync();

        Console.WriteLine($"Start listening for @{me.Username}");
    }

    InlineKeyboardMarkup bridgeChannelInlineKeyboard(long chatId)
    {
        var inlineKeyboard = new InlineKeyboardMarkup(new[]
                        {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData("تنظیم این کانال به عنوان Bridge", $"bridgeChannel/{chatId}")
                        }
                    });

        return inlineKeyboard;
    }
    static async void SendMessage(ITelegramBotClient bot, long chatId, string message, CancellationToken cancellationToken)
    {
        const int maxMessageLength = 4096; // Maximum message length for Telegram

        if (message.Length <= maxMessageLength)
        {
            await bot.SendTextMessageAsync(chatId, message, cancellationToken: cancellationToken);
        }
        else
        {
            // Split the message into chunks of maxMessageLength
            List<string> chunks = new List<string>();
            for (int i = 0; i < message.Length; i += maxMessageLength)
            {
                int length = Math.Min(maxMessageLength, message.Length - i);
                chunks.Add(message.Substring(i, length));
            }

            // Send each chunk separately
            foreach (string chunk in chunks)
            {
                await bot.SendTextMessageAsync(chatId, chunk, cancellationToken: cancellationToken);
            }
        }
    }

    async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            BotSettingsEntity botSettings = botSettingsService.GetBotSettings();

            ChannelPostHandler channelPostHandler = new(postService, botClient, update, cancellationToken, botSettings, logger);
            CommandHandler commandHandler = new(botClient, update, cancellationToken, botSettings, logger, mapper, botSettingsService);
            CallbackQueryHandler callbackQueryHandler = new(botClient, update, cancellationToken, botSettings, logger, mapper, botSettingsService, postService);


            List<long> ignoreList = channelService.GetDeactivatedChannels().Select(x => x.ChannelTelegramId).ToList();

            if (update.Type == UpdateType.ChannelPost && update.ChannelPost.Chat.Id == botSettings.BridgeChanneID)
            {
                var logMessage = $"Type: {update.Type} , {update.ChannelPost.Caption ?? update.ChannelPost.Text} , {update.ChannelPost.Chat.Username}";
                logger.LogInfo(logMessage);

                if (update.ChannelPost is not { } post)
                    return;

                var chatId = post.Chat.Id;
                var messageId = post.MessageId;

                Console.WriteLine($"Received a '{messageId}' message in chat {chatId}.");

                if (post.ForwardFromChat != null)
                    channelPostHandler.OnChannelPost(post, ignoreList);
            }
            else if (update.Type == UpdateType.CallbackQuery)
            {
                if (update.CallbackQuery is not { } message)
                    return;

                if (message.Data is not { } messageData)
                    return;

                Console.WriteLine($"Received a '{messageData}' message in chat");

                var newStatus = messageData;
                var data = messageData.Split("/");

                var chatId = message.Message.Chat.Id;
                var messageId = message.Message.MessageId;
                var FirstName = message.From.FirstName;
                var Username = message.From.Username ?? "";

                var logMessage = $"Type: {update.Type} , {FirstName} , {Username} , {messageData}";
                logger.LogInfo(logMessage);


                if (data[0] == "bridgeChannel")
                {
                    callbackQueryHandler.OnSetBridgeChannel(message, data[1]);
                }
                else
                {
                    if (data[0] == "accept")
                    {
                        callbackQueryHandler.OnAcceptPost(message);
                    }
                    else if (data[0] == "license_check")
                    {
                        callbackQueryHandler.OnLicenseCheckPost(message);
                    }
                    else if (data[0] == "set_Main_Alternative_Group_Topic")
                    {
                        callbackQueryHandler.OnSetMainAlternativeGroupTopic(message);
                    }
                    else if (data[0] == "set_license_check_topic")
                    {
                        callbackQueryHandler.OnSetLicenseCheckGroup(message);
                    }
                    else if (data[0] == "inspect_topic")
                    {
                        callbackQueryHandler.OnSetInspectTopicGroup(message);
                    }
                    else if (data[0] == "decline")
                    {
                        callbackQueryHandler.OnDeclinePost(message);
                    }
                    else if (data[0] == "more")
                    {
                        callbackQueryHandler.OnInspectMorePost(message);
                    }
                }

                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, newStatus, cancellationToken: cancellationToken);
            }
            else if (update.Type == UpdateType.Message)
            {
                if (update.Message is not { } message)
                    return;

                if (message.Text is not { } messageText)
                    return;

                var chatId = message.Chat.Id;

                var logMessage = $"Type: {update.Type} , {chatId} , {messageText} , {message.Chat.ActiveUsernames}";
                logger.LogInfo(logMessage);

                Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

                if (messageText.StartsWith("/set_inspect_topic"))
                {
                    commandHandler.OnSetInspectTopic(message);
                    return;
                }
                if (messageText.StartsWith("/set_license_check_topic"))
                {
                    commandHandler.OnSetLicenseCheckTopic(message);
                    return;
                }
                if (messageText.StartsWith("/set_forward_post_topic"))
                {
                    commandHandler.OnSetMainAlternativeGroupTopic(message);
                    return;
                }

                if (messageText.StartsWith("/set_maingroup"))
                {
                    commandHandler.OnSetMainGroup(message);
                    return;
                }
                if (messageText.StartsWith("/set_mainchannel"))
                {
                    commandHandler.OnSetMainChannel(message);
                    return;
                }
                if (messageText.StartsWith("/channel_list"))
                {
                    var results = channelService.GetChannelList();

                    string text = "";

                    foreach (var channel in results)
                    {
                        var activestatus = "✅";
                        var deactivestatus = "❌";

                        text += $"{channel.Name} - @{channel.Username}  {(channel.IsActive ? activestatus : deactivestatus)}\n";
                    }

                    SendMessage(botClient, chatId, text, cancellationToken);

                    return;
                }
            }
            else if (update.Type == UpdateType.MyChatMember && update.MyChatMember is { } chatMember)
            {
                if (chatMember.NewChatMember.Status == ChatMemberStatus.Administrator)
                {
                    var bridgeChatId = chatMember.Chat.Id;
                    var bridgeChannelMarkup = bridgeChannelInlineKeyboard(chatMember.Chat.Id);

                    Message markupMessage = await botClient.SendTextMessageAsync(
                                                chatId: bridgeChatId,
                                                text: "برای انتخاب این کانال به عنوان کانال میانی تایید کنید",
                                                replyMarkup: bridgeChannelMarkup,
                                                cancellationToken: cancellationToken);
                }
            }
            else
            {
                var logMessage2 = $"Type: {update.Type}";
                logger.LogInfo(logMessage2);
            }

        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            return;
        }
    }


    Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}