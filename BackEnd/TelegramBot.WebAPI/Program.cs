using System.Text.Json;
using AutoMapper;
using TelegramBot.Application.Interfaces;
using TelegramBot.Application.Services.BotSettings;
using TelegramBot.Application.Services.ChannelService;
using TelegramBot.Application.Services.PostService;
using TelegramBot.Bot;
using TelegramBot.Infrastructure;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddInfrastructureConfig();
// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });;

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure();
builder.Services.AddApplication();

builder.Services.AddAutoMapper(typeof(Program));


// builder.Services.AddScoped<IPostService, PostService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseInfrastructure();

// new Thread(() =>
// {
//     var scope = app.Services.CreateScope();
//     Thread.CurrentThread.IsBackground = true;

//     var telegramBot = new StartBot(
//         scope.ServiceProvider.GetRequiredService<IMapper>(),
//         scope.ServiceProvider.GetRequiredService<IPostService>(),
//         scope.ServiceProvider.GetRequiredService<IBotSettingsService>(),
//         scope.ServiceProvider.GetRequiredService<ILoggerService>(),
//         scope.ServiceProvider.GetRequiredService<IChannelService>()
//     );

//     telegramBot.Start();
// }).Start();


app.UseAuthorization();
app.MapControllers();

app.Run();