namespace TelegramBot.Core.Entities;

public class ChannelInfoEntity : BaseEntity
{
    public string? Name {get;set;}
    public string? Username {get;set;}
    public string? Description {get;set;}
    public string? Phone {get;set;}
    public long ChannelTelegramId {get;set;}
    public DateTime JoinDate {get;set;}
    public bool IsActive {get;set;}
}