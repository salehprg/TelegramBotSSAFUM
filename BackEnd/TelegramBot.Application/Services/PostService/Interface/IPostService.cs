using TelegramBot.Application.Interfaces;
using TelegramBot.Core.Entities;

namespace TelegramBot.Application.Services.PostService;

public interface IPostService {

    IRepository<PostInfoEntity> Repo();
    
    PostInfoEntity GetByPostId(int messageId);
    IQueryable<PostInfoEntity> GetByChatId(long chatId);
    IEnumerable<PostInfoEntity> GetEventsByTelegramId(long TelegramChatId , int TelegramPostId);
    PostInfoEntity SaveNewPost(PostInfoEntity postInfo);
    PostInfoEntity RemoveOldPostStatus(PostInfoEntity postInfo);
    PostInfoEntity ForwardToGroupPost(PostInfoEntity postInfo, PostInfoTelegramModel telegramIds);
    PostInfoEntity ForwardToChannelPost(PostInfoEntity postInfo);
    PostInfoEntity AcceptPost(PostInfoEntity postInfo);
    PostInfoEntity DeclinePost(PostInfoEntity postInfo);
    PostInfoEntity InspectPost(PostInfoEntity postInfo , PostInfoTelegramModel telegramIds);
    PostInfoEntity LicenseCheckPost(PostInfoEntity postInfo , PostInfoTelegramModel telegramIds);
    IEnumerable<PostInfoEvent> GetRemovableEvents(int postInfoId);
}