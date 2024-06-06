using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Omu.ValueInjecter;
using TelegramBot.Application.Services.BotSettings;
using TelegramBot.Core.Entities;

namespace TelegramBot.WebAPI.Controllers;

[ApiController]
[Route("api/bot/settings")]
public class BotSettingsController : ControllerBase
{
    private readonly IBotSettingsService _botSettingsService;
    private readonly IMapper mapper;

    public BotSettingsController(IBotSettingsService botSettingsService, IMapper mapper)
    {
        _botSettingsService = botSettingsService;
        this.mapper = mapper;
    }

    [HttpPost("set")]
    public ActionResult<BotSettingsEntity> SetBotSettings(BotSettingsModel botSettings)
    {
        var currentSetting = _botSettingsService.GetBotSettings();
        currentSetting.InjectFrom(botSettings);

        var updatedSettings = _botSettingsService.SetBotSettings(mapper.Map<BotSettingsModel>(currentSetting));
        return Ok(updatedSettings);
    }

    [HttpGet("get")]
    public ActionResult<BotSettingsEntity> GetBotSettings()
    {
        var botSettings = _botSettingsService.GetBotSettings();
        if (botSettings == null)
        {
            return NotFound();
        }
        return Ok(botSettings);
    }
}
