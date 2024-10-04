using Bold.Integration.Base.Services;

namespace Bold.Integration.Base.Middlewares;

public class EmailLogger(EmailService emailService) : ILogger
{
    IDisposable? ILogger.BeginScope<TState>(TState state)
    {
        return null;
    }
    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel == LogLevel.Error || logLevel == LogLevel.Critical;
    }
    public async void Log<TState>(LogLevel logLevel,
                                  EventId eventId,
                                  TState state,
                                  Exception? exception,
                                  Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var message = state+(exception?.Message is not null ? $"\n{exception.Message}" : string.Empty);
        await emailService.SendErrorAsync(message);
    }
}