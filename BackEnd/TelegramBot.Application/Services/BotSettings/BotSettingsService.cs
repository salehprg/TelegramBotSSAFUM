using AutoMapper;
using Omu.ValueInjecter;
using TelegramBot.Application.Interfaces;
using TelegramBot.Core.Entities;

namespace TelegramBot.Application.Services.BotSettings;

public class BotSettingsService : IBotSettingsService
{

    IRepository<BotSettingsEntity> botsettingsRepo;
    IMapper mapper;

    public BotSettingsService(IRepository<BotSettingsEntity> botsettingsRepo, IMapper mapper)
    {
        this.botsettingsRepo = botsettingsRepo;
        this.mapper = mapper;
    }

    public BotSettingsEntity GetBotSettings()
    {
        return botsettingsRepo.FirstOrDefault();
    }

    public BotSettingsEntity SetBotSettings(BotSettingsModel botSettings)
    {
        Console.WriteLine(botSettings.MainGroupID);
        
        var setting = GetBotSettings();

        setting.InjectFrom(botSettings);

        botsettingsRepo.Edit(setting);
        botsettingsRepo.SaveChanges();

        return setting;
    }
}