using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TelegramBot.Core.Entities;

namespace TelegramBot.WebAPI.Models
{
    public class PostInfoInputModel
    {
        public int Id { get; set; }
        public DateTime? SendDate { get; set; }
        public DateTime? ForwardToGroupDate { get; set; }
        public DateTime? ForwardToChannelDate { get; set; }
        public List<int> Telegram_mediagroup_postids { get; set; } = new List<int>();
        public int Media_groupId { get; set; }
        public int TelegramId_PostId { get; set; }
        public long TelegramId_ChatId { get; set; }
        public string? StatusChanged_Username { get; set; }
        public EnumPostStatus? PostStatus { get; set; }

        private class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<PostInfoInputModel, PostInfoEntity>().ReverseMap();
            }
        }
    }
}