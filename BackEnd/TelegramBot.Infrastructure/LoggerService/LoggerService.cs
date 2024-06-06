using TelegramBot.Application.Interfaces;
using TelegramBot.Infrastructure.Models;

namespace TelegramBot.Infrastructure;

public class LoggerService : ILoggerService
{
    IRepository<LogModel> _logRepository;

    public LoggerService(IRepository<LogModel> logRepository)
    {
        _logRepository = logRepository;
    }

    public void LogError(string message)
    {
        LogModel logModel = new LogModel()
        {
            Message = message,
            Type = "Error",
            LogTime = DateTime.UtcNow
        };

        _logRepository.Add(logModel);
        _logRepository.SaveChanges();
    }

    public void LogInfo(string message)
    {
        LogModel logModel = new LogModel()
        {
            Message = message,
            Type = "Info",
            LogTime = DateTime.UtcNow
        };

        _logRepository.Add(logModel);
        _logRepository.SaveChanges();
    }

    public void LogWarning(string message)
    {
        LogModel logModel = new LogModel()
        {
            Message = message,
            Type = "Warning",
            LogTime = DateTime.UtcNow
        };

        _logRepository.Add(logModel);
        _logRepository.SaveChanges();
    }
}