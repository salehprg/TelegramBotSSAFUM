namespace TelegramBot.Application.Interfaces;

public interface ILoggerService
{
    void LogInfo(string message);
    void LogError(string message);
    void LogWarning(string message);
}