using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TelegramBot.Core.Entities;

namespace TelegramBot.Application.Services.ChannelService.DTOModel
{
    public class ChannelInfoModel
    {
        public string? Name { get; set; }
        public string? Username { get; set; }
        public string? Description { get; set; }
        public string? Phone { get; set; }
        public long ChannelTelegramId { get; set; }

        private class Mapping : Profile{
            public Mapping(){
                CreateMap<ChannelInfoModel , ChannelInfoEntity>().ReverseMap();
            }
        }
    }
}