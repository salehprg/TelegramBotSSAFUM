using TelegramBot.Application.Interfaces;
using TelegramBot.Core.Entities;

namespace TelegramBot.Application.Services.PostService;

public class PostService : IPostService
{
    IRepository<PostInfoEntity> postRepository;
    IRepository<PostInfoEvent> postEventsRepository;
    public PostService(IRepository<PostInfoEntity> postRepository, IRepository<PostInfoEvent> postEventsRepository)
    {
        this.postRepository = postRepository;
        this.postEventsRepository = postEventsRepository;
    }

    public IQueryable<PostInfoEntity> GetAll()
    {
        return postRepository.GetAll();
    }

    void AddPostEvent(PostInfoEntity postInfo, PostInfoTelegramModel postInfoTelegramModel)
    {
        PostInfoEvent postInfoEvent = new()
        {
            EventDateTime = DateTime.UtcNow,
            PostInfoId = postInfo.Id,
            PostStatus = postInfo.PostStatus,
            TelegramId_ChatId = postInfoTelegramModel.TelegramId_ChatId,
            TelegramId_PostId = postInfoTelegramModel.TelegramId_PostId,
            Repliyed_TelegramId_ChatId = postInfoTelegramModel.Repliyed_TelegramId_ChatId,
            Repliyed_TelegramId_PostId = postInfoTelegramModel.Repliyed_TelegramId_PostId
        };

        postEventsRepository.Add(postInfoEvent);
        postEventsRepository.SaveChanges();
    }

    public PostInfoEntity AcceptPost(PostInfoEntity postInfo)
    {
        postInfo.PostStatus = EnumPostStatus.Accepted;

        var lastDate = GetAll().OrderByDescending(x => x.ForwardToChannelDate).Select(x => x.ForwardToChannelDate).FirstOrDefault();

        var sendDate = lastDate.AddMinutes(5);
        
        if((sendDate - DateTime.UtcNow).TotalMinutes < 5){
            sendDate = DateTime.UtcNow.AddMinutes(5);
        }

        postInfo.ForwardToChannelDate = sendDate;

        postRepository.Edit(postInfo);
        postRepository.SaveChanges();

        AddPostEvent(postInfo, new PostInfoTelegramModel());

        return postInfo;
    }

    public PostInfoEntity DeclinePost(PostInfoEntity postInfo)
    {
        postInfo.PostStatus = EnumPostStatus.Declined;

        postRepository.Edit(postInfo);
        postRepository.SaveChanges();

        AddPostEvent(postInfo, new PostInfoTelegramModel());

        return postInfo;
    }

    public PostInfoEntity SaveNewPost(PostInfoEntity postInfo)
    {
        postInfo.SendDate = DateTime.UtcNow;
        postInfo.PostStatus = EnumPostStatus.Pending;

        postRepository.Add(postInfo);
        postRepository.SaveChanges();

        AddPostEvent(postInfo, new PostInfoTelegramModel());

        return postInfo;
    }

    public IQueryable<PostInfoEntity> GetByChatId(long chatId)
    {
        return GetAll().Where(x => x.TelegramId_ChatId == chatId);
    }

    public PostInfoEntity GetByPostId(int messageId)
    {
        return GetAll().Where(x => x.Id == messageId).FirstOrDefault();
    }

    public IRepository<PostInfoEntity> Repo()
    {
        return postRepository;
    }

    public PostInfoEntity InspectPost(PostInfoEntity postInfo, PostInfoTelegramModel telegramIds)
    {
        postInfo.PostStatus = EnumPostStatus.InspectMore;

        postRepository.Edit(postInfo);
        postRepository.SaveChanges();

        AddPostEvent(postInfo, telegramIds);

        return postInfo;
    }

    public PostInfoEntity ForwardToGroupPost(PostInfoEntity postInfo, PostInfoTelegramModel telegramIds)
    {
        postInfo.PostStatus = EnumPostStatus.ForwardToGroup;
        postInfo.ForwardToGroupDate = DateTime.UtcNow;

        postRepository.Edit(postInfo);
        postRepository.SaveChanges();

        AddPostEvent(postInfo, telegramIds);

        return postInfo;
    }

    public PostInfoEntity ForwardToChannelPost(PostInfoEntity postInfo)
    {
        postInfo.ForwardToChannelDate = DateTime.UtcNow;

        postRepository.Edit(postInfo);
        postRepository.SaveChanges();

        return postInfo;
    }

    public PostInfoEntity LicenseCheckPost(PostInfoEntity postInfo, PostInfoTelegramModel telegramIds)
    {
        postInfo.PostStatus = EnumPostStatus.LicenseCheck;

        postRepository.Edit(postInfo);
        postRepository.SaveChanges();

        AddPostEvent(postInfo, telegramIds);

        return postInfo;
    }

    public IEnumerable<PostInfoEvent> GetRemovableEvents(int postInfoId)
    {
        var postInfo = GetByPostId(postInfoId);
        return postInfo.PostEvents.Where(x => x.PostStatus == EnumPostStatus.InspectMore || x.PostStatus == EnumPostStatus.ForwardToGroup || x.PostStatus == EnumPostStatus.LicenseCheck);
    }

    public IEnumerable<PostInfoEntity> GetEventsByTelegramId(long TelegramChatId, int TelegramPostId)
    {
        return GetAll().Where(x => x.TelegramId_ChatId == TelegramChatId && x.TelegramId_PostId == TelegramPostId
                                && x.PostStatus != EnumPostStatus.Removed && x.PostStatus != EnumPostStatus.Pending).AsEnumerable();
    }

    public PostInfoEntity RemoveOldPostStatus(PostInfoEntity postInfo)
    {
        postInfo.PostStatus = EnumPostStatus.Removed;

        postRepository.Edit(postInfo);
        postRepository.SaveChanges();

        AddPostEvent(postInfo, new PostInfoTelegramModel());

        return postInfo;
    }
}