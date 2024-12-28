using System.Net;
using System.Net.Mail;
using SecureAuthLib.Business.Abstract;

public class EmailService : IEmailService
{
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _smtpUser;
    private readonly string _smtpPass;

    public EmailService(string smtpServer, int smtpPort, string smtpUser, string smtpPass)
    {
        _smtpServer = smtpServer;
        _smtpPort = smtpPort;
        _smtpUser = smtpUser;
        _smtpPass = smtpPass;
    }

    // Sends an activation email
    public void SendActivationEmail(string email, string activationLink)
    {
        var subject = "Account Activation";
        var body = $"Please activate your account by clicking the following link: {activationLink}";
        SendEmail(email, subject, body);
    }

    // Sends a password reset email
    public void SendPasswordResetEmail(string email, string resetLink)
    {
        var subject = "Password Reset";
        var body = $"You can reset your password by clicking the following link: {resetLink}";
        SendEmail(email, subject, body);
    }

    // Sends an email using SMTP
    private void SendEmail(string toEmail, string subject, string body)
    {
        using (var client = new SmtpClient(_smtpServer, _smtpPort))
        {
            client.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
            client.EnableSsl = true;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpUser),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);

            client.Send(mailMessage);
        }
    }
}
