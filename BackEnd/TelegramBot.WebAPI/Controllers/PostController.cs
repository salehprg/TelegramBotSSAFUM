using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TelegramBot.Application.Services.PostService;
using TelegramBot.Core.Entities;
using TelegramBot.WebAPI.Models;

[Route("api/posts")]
[ApiController]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly IMapper mapper;

    public PostController(IPostService postService, IMapper mapper)
    {
        _postService = postService;
        this.mapper = mapper;
    }

    [HttpGet("{postId}")]
    public ActionResult<PostInfoEntity> GetById(int postId)
    {
        var post = _postService.GetByPostId(postId);
        if (post == null)
        {
            return NotFound();
        }
        return Ok(mapper.Map<PostInfoDTO>(post));
    }

    [HttpGet("chat/{chatId}")]
    public ActionResult<IEnumerable<PostInfoEntity>> GetByChatId(long chatId)
    {
        var posts = _postService.GetByChatId(chatId).ToList();

        var result = mapper.Map<List<PostInfoDTO>>(posts);
        return Ok(result);
    }

    [HttpGet("events")]
    public ActionResult<IEnumerable<PostInfoEntity>> GetEvents(long telegramChatId, int telegramPostId)
    {
        var events = _postService.GetEventsByTelegramId(telegramChatId, telegramPostId).ToList();

        var result = mapper.Map<List<PostInfoDTO>>(events);
        return Ok(result);
    }

    [HttpPost("new")]
    public ActionResult<PostInfoEntity> Create(PostInfoInputModel _postInfo)
    {
        var postInfo = mapper.Map<PostInfoEntity>(_postInfo);

        var savedPost = _postService.SaveNewPost(postInfo);
        savedPost.PostEvents = [];

        return Ok(savedPost);
    }

    [HttpPost("remove")]
    public ActionResult<PostInfoEntity> RemoveStatus(PostInfoInputModel _postInfo)
    {
        var postInfo = _postService.GetByPostId(_postInfo.Id);

        var removedPost = _postService.RemoveOldPostStatus(postInfo);

        var result = mapper.Map<PostInfoDTO>(removedPost);

        return Ok(result);
    }

    [HttpPost("forward/group")]
    public ActionResult<PostInfoEntity> ForwardToGroup(PostInputModel dataModel)
    {
        var postInfo = _postService.GetByPostId(dataModel.postInfo.Id);

        var forwardedPost = _postService.ForwardToGroupPost(postInfo, dataModel.telegramIds);
        forwardedPost.PostEvents = [];

        return Ok(forwardedPost);
    }

    [HttpPost("forward/channel")]
    public ActionResult<PostInfoEntity> ForwardToChannel(PostInfoInputModel _postInfo)
    {
        var postInfo = _postService.GetByPostId(_postInfo.Id);

        var forwardedPost = _postService.ForwardToChannelPost(postInfo);
        forwardedPost.PostEvents = [];

        return Ok(forwardedPost);
    }

    [HttpPost("accept")]
    public ActionResult<PostInfoEntity> Accept(PostInfoInputModel _postInfo)
    {
        var postInfo = _postService.GetByPostId(_postInfo.Id);

        var acceptedPost = _postService.AcceptPost(postInfo);
        acceptedPost.PostEvents = [];

        return Ok(acceptedPost);
    }

    [HttpPost("decline")]
    public ActionResult<PostInfoEntity> Decline(PostInfoInputModel _postInfo)
    {
        var postInfo = _postService.GetByPostId(_postInfo.Id);

        var declinedPost = _postService.DeclinePost(postInfo);
        declinedPost.PostEvents = [];

        return Ok(declinedPost);
    }

    [HttpPost("inspect")]
    public ActionResult<PostInfoEntity> Inspect(PostInputModel dataModel)
    {
        var postInfo = _postService.GetByPostId(dataModel.postInfo.Id);

        var inspectedPost = _postService.InspectPost(postInfo, dataModel.telegramIds);
        inspectedPost.PostEvents = [];

        return Ok(inspectedPost);
    }

    [HttpPost("license/check")]
    public ActionResult<PostInfoEntity> LicenseCheck(PostInputModel dataModel)
    {
        var postInfo = _postService.GetByPostId(dataModel.postInfo.Id);

        var checkedPost = _postService.LicenseCheckPost(postInfo, dataModel.telegramIds);
        checkedPost.PostEvents = [];

        return Ok(checkedPost);
    }

    [HttpGet("{postInfoId}/removable-events")]
    public ActionResult<IEnumerable<PostInfoEvent>> GetRemovableEvents(int postInfoId)
    {
        var removableEvents = _postService.GetRemovableEvents(postInfoId).ToList();

        var result = mapper.Map<List<PostInfoEventDTO>>(removableEvents);

        return Ok(result);
    }
}

public class PostInputModel
{
    public PostInfoInputModel postInfo { get; set; }
    public PostInfoTelegramModel telegramIds { get; set; }

}
