using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TelegramBot.Core.Entities;
using TelegramBot.Infrastructure;

public static class SeedData
{
    public static void SeedSettings(DatabaseContext databaseContext)
    {
        var settings = databaseContext.BotSettings.Count();

        if (settings == 0)
        {
            var defaultSettings = new BotSettingsEntity()
            {
                DelayBetweenPosts = 10,
                MainChanneID = 0,
                MainChannelUsername = "",
                MainGroupID = 0,
                MainGroupUsername = "",
                ReminderAccountDelay = new TimeSpan(),
                ReminderPostDelay = new TimeSpan(),
                botToken = ""
            };

            databaseContext.BotSettings.Add(defaultSettings);
            databaseContext.SaveChanges();
        }

    }
}