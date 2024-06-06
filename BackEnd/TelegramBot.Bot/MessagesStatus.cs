namespace TelegramBot.Bot;

public class MessagesStatus
{
    public static string AcceptMessage(string name , string username){
        return $"وضعیت : تایید شده ✅\n توسط : {name} - @{username}";
    }

    public static string DeclineMessage(string name , string username){
        return $"وضعیت : رد شده ❌\n توسط : {name} - @{username}";
    }

    public static string MoreMessage(string name , string username){
        return $"وضعیت : ارسال برای بازرسی 🔍\n توسط : {name} - @{username}";
    }

    public static string LicenseCheckMessage(string name , string username){
        return $"وضعیت : ارسال برای بررسی مجوز ⚠️\n توسط : {name} - @{username}";
    }
}