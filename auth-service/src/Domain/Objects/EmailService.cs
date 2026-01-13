using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace sevaLK_service_auth.Domain.Objects;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpUsername;
    private readonly string _smtpPassword;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly string _frontendUrl;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
        _smtpHost = configuration["SmtpHost"] ?? "smtp.gmail.com";
        _smtpPort = int.Parse(configuration["SmtpPort"] ?? "587");
        _smtpUsername = configuration["SmtpUsername"] ?? "";
        _smtpPassword = configuration["SmtpPassword"] ?? "";
        _fromEmail = configuration["FromEmail"] ?? "";
        _fromName = configuration["FromName"] ?? "SevaLK";
        _frontendUrl = configuration["FrontendUrl"] ?? "http://localhost:5173";
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string resetToken, string userName)
    {
        var subject = "Password Reset Request - SevaLK";
        var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border-radius: 5px; margin-top: 20px; }}
        .code-box {{ 
            background-color: #f0f0f0; 
            border: 2px solid #4CAF50;
            padding: 20px; 
            text-align: center;
            border-radius: 5px;
            margin: 20px 0;
            font-family: monospace;
            font-size: 18px;
            letter-spacing: 2px;
            font-weight: bold;
            color: #333;
        }}
        .footer {{ margin-top: 30px; font-size: 12px; color: #777; text-align: center; }}
        .warning {{ background-color: #fff3cd; border-left: 4px solid #ffc107; padding: 10px; margin: 20px 0; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Password Reset Request</h1>
        </div>
        <div class='content'>
            <p>Hello {userName},</p>
            
            <p>We received a request to reset your password for your SevaLK account. If you didn't make this request, please ignore this email.</p>
            
            <p>Use the following reset code to reset your password:</p>
            
            <div class='code-box'>
                {resetToken}
            </div>
            
            <p style='text-align: center; color: #666; font-size: 14px;'>Copy this code and paste it in the password reset form.</p>
            
            <div class='warning'>
                <strong>⚠️ Security Notice:</strong> This code will expire in 1 hour. For your security, never share this code with anyone.
            </div>
            
            <p>If you didn't request a password reset, please ignore this email or contact support if you have concerns.</p>
        </div>
        <div class='footer'>
            <p>&copy; {DateTime.UtcNow.Year} SevaLK. All rights reserved.</p>
            <p>This is an automated email, please do not reply.</p>
        </div>
    </div>
</body>
</html>";

        await SendEmailAsync(toEmail, subject, body, isHtml: true);
    }

    public async Task SendWelcomeEmailAsync(string toEmail, string userName)
    {
        var subject = "Welcome to SevaLK!";
        var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border-radius: 5px; margin-top: 20px; }}
        .footer {{ margin-top: 30px; font-size: 12px; color: #777; text-align: center; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Welcome to SevaLK!</h1>
        </div>
        <div class='content'>
            <p>Hello {userName},</p>
            
            <p>Thank you for registering with SevaLK! Your account has been successfully created.</p>
            
            <p>You can now log in and start using our services.</p>
            
            <p>If you have any questions or need assistance, please don't hesitate to contact our support team.</p>
        </div>
        <div class='footer'>
            <p>&copy; {DateTime.UtcNow.Year} SevaLK. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

        await SendEmailAsync(toEmail, subject, body, isHtml: true);
    }

    public async Task SendVerificationEmailAsync(string toEmail, Guid userId, string userName)
    {
        var subject = "Verify Your Email - QuickWheels";
        // Backend verification endpoint that will process the verification
        var backendVerificationUrl = $"{_frontendUrl}/api/auth/verify-email?id={userId}";
        var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border-radius: 5px; margin-top: 20px; }}
        .button {{ display: inline-block; background-color: #4CAF50; color: white; padding: 15px 30px; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .footer {{ margin-top: 30px; font-size: 12px; color: #777; text-align: center; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Welcome to QuickWheels!</h1>
        </div>
        <div class='content'>
            <p>Hello <strong>{userName}</strong>,</p>
            <p>Thank you for registering! Please verify your email by clicking the button below:</p>
            <div style='text-align: center;'>
                <a href='{backendVerificationUrl}' class='button'>Verify Email</a>
            </div>
            <p>Or copy this link: {backendVerificationUrl}</p>
        </div>
        <div class='footer'>
            <p>&copy; {DateTime.UtcNow.Year} QuickWheels. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

        await SendEmailAsync(toEmail, subject, body, isHtml: true);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = false)
    {
        try
        {
            using var message = new MailMessage();
            message.From = new MailAddress(_fromEmail, _fromName);
            message.To.Add(new MailAddress(toEmail));
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = isHtml;

            using var smtpClient = new SmtpClient(_smtpHost, _smtpPort);
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);

            await smtpClient.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            // Log the error (in production, use proper logging)
            Console.WriteLine($"Failed to send email to {toEmail}: {ex.Message}");
            throw new Exception($"Failed to send email: {ex.Message}", ex);
        }
    }
}
