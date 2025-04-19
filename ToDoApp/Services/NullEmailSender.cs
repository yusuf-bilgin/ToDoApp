using Microsoft.AspNetCore.Identity.UI.Services;

namespace ToDoApp.Services;

public class NullEmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // Henüz gerçek e-mail gönderimi yapılmıyor
        Console.WriteLine($"[FAKE EMAIL] To: {email} | Subject: {subject}");
        return Task.CompletedTask;
    }
}
