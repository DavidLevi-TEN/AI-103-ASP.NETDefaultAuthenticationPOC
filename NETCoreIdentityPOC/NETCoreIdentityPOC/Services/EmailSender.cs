using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;


namespace NETCoreIdentityPOC.Services;


public class EmailSender : IEmailSender
{
    private readonly ILogger<EmailSender> _logger;


    public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor, ILogger<EmailSender> logger)
    {
        Options = optionsAccessor.Value;
        _logger = logger;
    }

    public AuthMessageSenderOptions Options { get; }
    // In a real-life scenario, this ^ would be set up with a secrets manager

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        if (string.IsNullOrEmpty(Options.SendGridKey))
        {
            throw new Exception("Send Grid Key is null.");
        }
        await Execute(Options.SendGridKey, subject, message, email);
    }


    private async Task Execute(string apiKey, string subject, string message, string email)
    {
        var client = new SendGridClient(apiKey);

        var msg = new SendGridMessage()
        {
            From = new EmailAddress("test@outlook.com", "Password Recovery"),
            Subject = subject,
            PlainTextContent = message,
            HtmlContent = message
        };
        msg.AddTo(new EmailAddress(email));

        msg.SetClickTracking(false, false);

        var response = await client.SendEmailAsync(msg);
        _logger.LogInformation(response.IsSuccessStatusCode ? $"Email to {email} has been successfully queued."
            : $"Sending failure email to {email}");
    }
}
