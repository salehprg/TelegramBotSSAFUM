using System.ComponentModel.DataAnnotations.Schema;

namespace TelegramBot.Core.Entities;

public class PostInfoEvent : BaseEntity
{
    public DateTime EventDateTime {get;set;}
    public int PostInfoId {get;set;}
    public int TelegramId_PostId {get;set;}
    public long TelegramId_ChatId {get;set;}

    public int Repliyed_TelegramId_PostId {get;set;}
    public long Repliyed_TelegramId_ChatId {get;set;}
    public EnumPostStatus PostStatus {get;set;}


    [ForeignKey("PostInfoId")]
    public PostInfoEntity postInfoEntity {get;set;}
}