using Bold.Integration.Base.Services;

namespace Bold.Integration.Base.Middlewares;

public class EmailLoggerProvider(EmailService emailService) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new EmailLogger(emailService);
    }
    public void Dispose()
    { }
}