using System.Diagnostics;
using Bold.Integration.Base.Configuration;
using Bold.Integration.Base.Observability;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Bold.Integration.Base.Services;

public class EmailService(SendGridSettings settings)
{
    private readonly SendGridClient _client = new(settings.ApiKey);
    public Task<bool> SendErrorAsync(string error)
    {
        using var activity = Diagnostics.StartActivity("Sending error email");
        var topic = "Error en la integración con Bold";
        var html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>{topic}</title>
</head>
<body style='font-family: Arial, sans-serif; margin: 0; padding: 0; background-color: #f3f4f6;'>
    <table width='100%' border='0' cellspacing='0' cellpadding='0' style='margin: 0 auto;'>
        <tr>
            <td align='center' style='padding: 50px;'>
                <table border='0' cellspacing='0' cellpadding='0' style='background-color: #ffffff; border-radius: 8px; overflow: hidden; max-width: 500px; width: 100%;'>
                    <tr>
                        <td align='center' style='padding: 40px;'>
                            <img src='https://bold-factory.com/images/logo.png' alt='Bold Logo' style='max-width: 300px; width: 100%;'>
                        </td>
                    </tr>
                    <tr>
                        <td align='center' style='padding: 0 40px 20px; color: #555555; font-size: 18px;'>
                            {error}
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
        return SendEmailAsync(toEmail: settings.Destination,
                              subject: topic,
                              html: html,
                              fromEmail: "integrations@bold-factory.com",
                              fromName: "Bold Factory - Integraciones",
                              cc: "support@bold-factory.com",
                              activity: activity);
    }
    private async Task<bool> SendEmailAsync(string toEmail,
                                            string subject,
                                            string html,
                                            string fromEmail,
                                            string fromName,
                                            string? cc = null,
                                            string? bcc = null,
                                            Activity? activity = null)
    {
        var from = new EmailAddress(email: fromEmail, name: fromName);
        var msg = new SendGridMessage
        {
            From = from,
            Subject = subject,
            HtmlContent = html
        };
        if (!string.IsNullOrEmpty(toEmail))
        {
            var toEmails = toEmail.Split(separator: ';', options: StringSplitOptions.RemoveEmptyEntries);
            foreach (var email in toEmails)
            {
                msg.AddTo(new EmailAddress(email.Trim()));
            }
        }
        if (!string.IsNullOrEmpty(cc))
        {
            var ccEmails = cc.Split(separator: ';', options: StringSplitOptions.RemoveEmptyEntries);
            foreach (var email in ccEmails)
            {
                msg.AddCc(new EmailAddress(email.Trim()));
            }
        }

        if (!string.IsNullOrEmpty(bcc))
        {
            var bccEmails = bcc.Split(separator: ';', options: StringSplitOptions.RemoveEmptyEntries);
            foreach (var email in bccEmails)
            {
                msg.AddBcc(new EmailAddress(email.Trim()));
            }
        }
        var response = await _client.SendEmailAsync(msg);
        var body = await response.Body.ReadAsStringAsync();
        activity?.AddTag(key: "responseBody", value: body);
        return response.IsSuccessStatusCode;
    }
}