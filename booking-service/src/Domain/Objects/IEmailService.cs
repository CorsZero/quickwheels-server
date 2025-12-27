namespace booking_service.Domain.Objects;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string toEmail, string resetToken, string userName);
    Task SendWelcomeEmailAsync(string toEmail, string userName);
}
