namespace TelegramBot.Core.Entities;

public class PostInfoEntity : BaseEntity
{
    public DateTime SendDate {get;set;}
    public DateTime ForwardToGroupDate {get;set;}
    public DateTime ForwardToChannelDate {get;set;}
    public List<int> Telegram_mediagroup_postids {get;set;}
    public int Media_groupId {get;set;}
    public int TelegramId_PostId {get;set;}
    public long TelegramId_ChatId {get;set;}
    public string? StatusChanged_Username {get;set;}
    public EnumPostStatus PostStatus {get;set;}

    public IList<PostInfoEvent> PostEvents {get;set;}

}