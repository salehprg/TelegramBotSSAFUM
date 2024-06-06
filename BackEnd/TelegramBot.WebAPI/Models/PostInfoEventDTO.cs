using System.ComponentModel.DataAnnotations.Schema;
using AutoMapper;

namespace TelegramBot.Core.Entities;

public class PostInfoEventDTO
{
    public int Id { get; set; }
    public DateTime EventDateTime {get;set;}
    public int PostInfoId {get;set;}
    public int TelegramId_PostId {get;set;}
    public long TelegramId_ChatId {get;set;}

    public int Repliyed_TelegramId_PostId {get;set;}
    public long Repliyed_TelegramId_ChatId {get;set;}
    public EnumPostStatus PostStatus {get;set;}

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<PostInfoEvent, PostInfoEventDTO>().ReverseMap();
        }
    }
    
}