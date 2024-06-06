using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.Bot
{
    public static class Utility
    {
        public static InlineKeyboardMarkup getInlineKeyboard(int messageId)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
                            {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData("تایید ✅", $"accept/{messageId}"),
                            InlineKeyboardButton.WithCallbackData("رد ❌", $"decline/{messageId}"),
                        },
                        new []{
                            InlineKeyboardButton.WithCallbackData("بررسی 🔍", $"more/{messageId}"),
                            InlineKeyboardButton.WithCallbackData("بررسی مجوز ⚠️", $"license_check/{messageId}"),
                        }
                    });

            return inlineKeyboard;
        }

        public static InlineKeyboardMarkup getInspectMoreInlineKeyboard(int messageId)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
                            {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData("تایید ✅", $"accept/{messageId}"),
                            InlineKeyboardButton.WithCallbackData("رد ❌", $"decline/{messageId}")
                        }
                    });

            return inlineKeyboard;
        }

        public static InlineKeyboardMarkup getLicenseCheckInlineKeyboard(int messageId)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
                            {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData("تایید ✅", $"accept/{messageId}"),
                            InlineKeyboardButton.WithCallbackData("رد ❌", $"decline/{messageId}")
                        }
                    });

            return inlineKeyboard;
        }


        public static InlineKeyboardMarkup setInspectTopic_InlineKeyboard(long chatId, int messageId)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
                            {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData("انتخاب به عنوان گروه بازرسی 🔍", $"inspect_topic/{chatId}/{messageId}"),
                        }
                    });

            return inlineKeyboard;
        }

        public static InlineKeyboardMarkup setLicenseCheckTopic_InlineKeyboard(long chatId, int messageId)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
                            {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData("انتخاب به عنوان گروه بررسی مجوز ⚠️", $"set_license_check_topic/{chatId}/{messageId}"),
                        }
                    });

            return inlineKeyboard;
        }

        public static InlineKeyboardMarkup setMainAlternativeGroupTopic_InlineKeyboard(long chatId, int messageId)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
                            {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData("انتخاب به عنوان گروه ارسال پست ها 📩", $"set_Main_Alternative_Group_Topic/{chatId}/{messageId}"),
                        }
                    });

            return inlineKeyboard;
        }
    }
}