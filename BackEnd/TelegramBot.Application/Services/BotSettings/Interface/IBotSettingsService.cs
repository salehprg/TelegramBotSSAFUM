using TelegramBot.Core.Entities;

namespace TelegramBot.Application.Services.BotSettings;

public interface IBotSettingsService{
    BotSettingsEntity SetBotSettings(BotSettingsModel botSettings);
    BotSettingsEntity GetBotSettings();
}