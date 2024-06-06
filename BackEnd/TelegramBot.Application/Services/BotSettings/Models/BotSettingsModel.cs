using AutoMapper;
using TelegramBot.Core.Entities;

namespace TelegramBot.Application.Services.BotSettings;

public class BotSettingsModel
{
    public long AlternativeMainGroupID { get; set; }
    public int AlternativeMainGroupTopicID { get; set; }

    public long LicenseGroupID { get; set; }
    public int LicenseGroupTopicID { get; set; }

    public long InspectGroupID { get; set; }
    public int InspectGroupTopicID { get; set; }
    public long BridgeChanneID { get; set; }
    public long MainChanneID { get; set; }
    public long MainGroupID { get; set; }
    public string? MainChannelUsername { get; set; }
    public string? MainGroupUsername { get; set; }
    public float DelayBetweenPosts { get; set; }
    public TimeSpan ReminderPostDelay { get; set; }
    public TimeSpan ReminderAccountDelay { get; set; }

    private class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BotSettingsModel, BotSettingsEntity>().ReverseMap();
        }
    }
}