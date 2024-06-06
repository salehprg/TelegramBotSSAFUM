using TelegramBot.Core.Entities;

namespace TelegramBot.Infrastructure.Models;

public class LogModel : BaseEntity
{
    public string Message { get; set; }
    public string Type { get; set; }
    public DateTime LogTime { get; set; }
}