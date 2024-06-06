
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TelegramBot.Application.Interfaces;
using TelegramBot.Core.Entities;
using TelegramBot.Infrastructure.Models;

namespace TelegramBot.Infrastructure;

public static class DependencyInjection
{

    public static void AddInfrastructureConfig(this IConfiguration configuration)
    {
        var conStr = configuration.GetConnectionString("constr_Release");

#if DEBUG
        conStr = configuration.GetConnectionString("constr_Debug");
#endif

        Config.ConnectionString = conStr;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(DependencyInjection));
        
        services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(Config.ConnectionString));

        services.AddScoped<IRepository<PostInfoEvent> , RepositoryService<PostInfoEvent>>();
        services.AddScoped<IRepository<PostInfoEntity> , RepositoryService<PostInfoEntity>>();
        services.AddScoped<IRepository<BotSettingsEntity> , RepositoryService<BotSettingsEntity>>();
        services.AddScoped<IRepository<ChannelInfoEntity> , RepositoryService<ChannelInfoEntity>>();

        services.AddScoped<IRepository<LogModel> , RepositoryService<LogModel>>();
        services.AddScoped<ILoggerService , LoggerService>();

        return services;
    }

    public static void UseInfrastructure(this IApplicationBuilder builder)
    {
        using var scope = builder.ApplicationServices.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        db.Database.Migrate();

        SeedData.SeedSettings(db);
    }
}