using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using TelegramBot.Application.Services.ChannelService;
using TelegramBot.Application.Services.ChannelService.DTOModel;
using TelegramBot.Core.Entities;

namespace TelegramBot.WebAPI.Controllers;

[ApiController]
[Route("api/channels")]
public class ChannelController : ControllerBase
{
    private readonly IChannelService _channelService;

    public ChannelController(IChannelService channelService)
    {
        _channelService = channelService;
    }

    [HttpGet("{channelId}")]
    public ActionResult<ChannelInfoEntity> GetById(int channelId)
    {
        var channel = _channelService.GetById(channelId);
        if (channel == null)
        {
            return NotFound();
        }
        return Ok(channel);
    }

    [HttpGet("telegram/{chatId}")]
    public ActionResult<ChannelInfoEntity> GetByTelegramChannelId(long chatId)
    {
        var channel = _channelService.GetByTelegramChannelId(chatId);
        if (channel == null)
        {
            return NotFound();
        }
        return Ok(channel);
    }

    [HttpPost("add")]
    public ActionResult<ChannelInfoEntity> AddChannel(ChannelInfoModel channelInfo)
    {
        var addedChannel = _channelService.AddChannel(channelInfo);
        return Ok(addedChannel);
    }

    [HttpPost("add-multiple")]
    public ActionResult<List<ChannelInfoEntity>> AddChannel(List<ChannelInfoModel> channelInfo)
    {
        var addedChannels = _channelService.AddChannel(channelInfo);
        return Ok(addedChannels);
    }
    
    [HttpPut("update-multiple")]
    public ActionResult<List<ChannelInfoEntity>> UpdateChannels(List<ChannelInfoModel> channelInfo)
    {
        var updatedChannels = _channelService.UpdateChannels(channelInfo);
        return Ok(updatedChannels);
    }

    [HttpPut("deactivate/{telegramChannelId}")]
    public ActionResult<ChannelInfoEntity> DeactiveChannel(long telegramChannelId)
    {
        var deactivatedChannel = _channelService.DeactiveChannel(telegramChannelId);
        return Ok(deactivatedChannel);
    }

    [HttpGet("deactivated")]
    public ActionResult<IEnumerable<ChannelInfoEntity>> GetDeactivatedChannels()
    {
        var deactivatedChannels = _channelService.GetDeactivatedChannels().ToList();
        if (deactivatedChannels.Count == 0)
        {
            return NotFound();
        }
        return Ok(deactivatedChannels);
    }

    [HttpPut("edit")]
    public ActionResult<ChannelInfoEntity> EditChannel(ChannelInfoEntity channelInfo)
    {
        var editedChannel = _channelService.EditChannel(channelInfo);
        return Ok(editedChannel);
    }

    [HttpDelete("remove/{channelId}")]
    public ActionResult<ChannelInfoEntity> RemoveChannel(int channelId)
    {
        var removedChannel = _channelService.RemoveChannel(channelId);
        return Ok(removedChannel);
    }

    [HttpGet("list")]
    public ActionResult<IEnumerable<ChannelInfoEntity>> GetChannelList()
    {
        var channelList = _channelService.GetChannelList().ToList();
        if (channelList.Count == 0)
        {
            return NotFound();
        }
        return Ok(channelList);
    }
}
