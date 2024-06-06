using System.ComponentModel.DataAnnotations.Schema;

namespace TelegramBot.Core.Entities;

public class PostInfoTelegramModel
{
    public int TelegramId_PostId {get;set;}
    public long TelegramId_ChatId {get;set;}

    public int Repliyed_TelegramId_PostId {get;set;}
    public long Repliyed_TelegramId_ChatId {get;set;}
}