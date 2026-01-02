namespace sevaLK_service_auth.Domain.Objects;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string toEmail, string resetToken, string userName);
    Task SendWelcomeEmailAsync(string toEmail, string userName);
    Task SendVerificationEmailAsync(string toEmail, Guid userId, string userName);
}
