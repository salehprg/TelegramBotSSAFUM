using AutoMapper;
using TelegramBot.Application.Interfaces;
using TelegramBot.Application.Services.ChannelService.DTOModel;
using TelegramBot.Core.Entities;

namespace TelegramBot.Application.Services.ChannelService;

public class ChannelService : IChannelService
{
    IRepository<ChannelInfoEntity> channelRepository;
    IMapper mapper;
    public ChannelService(IRepository<ChannelInfoEntity> channelRepository, IMapper mapper)
    {
        this.channelRepository = channelRepository;
        this.mapper = mapper;
    }

    public ChannelInfoEntity AddChannel(ChannelInfoModel _channelInfo)
    {
        ChannelInfoEntity channelInfo = mapper.Map<ChannelInfoEntity>(_channelInfo);

        channelInfo.JoinDate = DateTime.UtcNow;
        channelInfo.IsActive = true;

        channelRepository.Add(channelInfo);
        channelRepository.SaveChanges();

        return channelInfo;
    }
    public List<ChannelInfoEntity> AddChannel(List<ChannelInfoModel> channelInfo)
    {
        var oldChannelIds = channelRepository.GetAll().Select(x => x.ChannelTelegramId).ToList();
        var newChannels = channelInfo.Where(x => !oldChannelIds.Contains(x.ChannelTelegramId)).ToList();

        List<ChannelInfoEntity> channels = mapper.Map<List<ChannelInfoEntity>>(newChannels);

        foreach (var channel in channels)
        {
            channel.JoinDate = DateTime.UtcNow;
            channel.IsActive = true;
        }

        channelRepository.Add(channels);

        return channels;
    }

    public List<ChannelInfoEntity> UpdateChannels(List<ChannelInfoModel> channelInfo)
    {
        var oldChannelIds = channelRepository.GetAll().Select(x => x.ChannelTelegramId).ToList();
        var newChannelIds = channelInfo.Select(x => x.ChannelTelegramId).ToList();

        var newChannels = channelInfo.Where(x => !oldChannelIds.Contains(x.ChannelTelegramId)).ToList();
        var removedChannels = channelRepository.GetAll().Where(x => !newChannelIds.Contains(x.ChannelTelegramId)).ToList();

        channelRepository.StartTransaction();

        channelRepository.RemoveRange(removedChannels);

        AddChannel(newChannels);

        channelRepository.CommitTransaction();

        return channelRepository.GetAll().ToList();
    }

    public ChannelInfoEntity EditChannel(ChannelInfoEntity channelInfo)
    {
        channelRepository.Edit(channelInfo);
        channelRepository.SaveChanges();

        return channelInfo;
    }

    public ChannelInfoEntity GetById(int channelId)
    {
        return channelRepository.GetById(channelId);
    }

    public ChannelInfoEntity GetByTelegramChannelId(long chatId)
    {
        return channelRepository.GetAll().Where(x => x.ChannelTelegramId == chatId).FirstOrDefault();
    }

    public IQueryable<ChannelInfoEntity> GetChannelList()
    {
        return channelRepository.GetAll();
    }

    public ChannelInfoEntity RemoveChannel(int channelId)
    {
        if (channelRepository.GetAll().Where(x => x.Id == channelId).FirstOrDefault() is { } channel)
        {
            channelRepository.Remove(channel);
            channelRepository.SaveChanges();

            return channel;
        }

        return null;
    }

    public IRepository<ChannelInfoEntity> Repo()
    {
        return channelRepository;
    }

    public ChannelInfoEntity DeactiveChannel(long telegramChannelId)
    {
        var channel = GetByTelegramChannelId(telegramChannelId);

        channel.IsActive = false;
        channelRepository.Edit(channel);

        return channel;
    }

    public IEnumerable<ChannelInfoEntity> GetDeactivatedChannels()
    {
        return channelRepository.GetAll().Where(x => x.IsActive == false);
    }
}