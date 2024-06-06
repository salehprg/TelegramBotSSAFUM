using TelegramBot.Application.Interfaces;
using TelegramBot.Application.Services.ChannelService.DTOModel;
using TelegramBot.Core.Entities;

namespace TelegramBot.Application.Services.ChannelService;

public interface IChannelService
{

    IRepository<ChannelInfoEntity> Repo();

    ChannelInfoEntity GetById(int channelId);
    ChannelInfoEntity GetByTelegramChannelId(long chatId);
    ChannelInfoEntity AddChannel(ChannelInfoModel channelInfo);
    List<ChannelInfoEntity> AddChannel(List<ChannelInfoModel> channelInfo);
    List<ChannelInfoEntity> UpdateChannels(List<ChannelInfoModel> channelInfo);
    ChannelInfoEntity DeactiveChannel(long telegramChannelId);
    IEnumerable<ChannelInfoEntity> GetDeactivatedChannels();
    ChannelInfoEntity EditChannel(ChannelInfoEntity channelInfo);
    ChannelInfoEntity RemoveChannel(int channelId);
    IQueryable<ChannelInfoEntity> GetChannelList();
}