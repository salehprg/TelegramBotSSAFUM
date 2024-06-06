using Microsoft.Extensions.DependencyInjection;
using TelegramBot.Application.Services.BotSettings;
using TelegramBot.Application.Services.ChannelService;
using TelegramBot.Application.Services.PostService;

public static class DependencyInjection
{

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(DependencyInjection));
        
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<IBotSettingsService, BotSettingsService>();
        services.AddScoped<IChannelService, ChannelService>();

        return services;
    }
}