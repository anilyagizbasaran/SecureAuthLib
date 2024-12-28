using Moq;
using SecureAuthLib.Business.Abstract;
using System.Net.Mail;
using Xunit;

public class EmailServiceTests
{
    private readonly Mock<IEmailService> _mockEmailService;

    public EmailServiceTests()
    {
        _mockEmailService = new Mock<IEmailService>();
    }

    [Fact]
    public void SendActivationEmail_ShouldSendEmail()
    {
        // Arrange
        var email = "test@example.com";
        var activationLink = "http://example.com/activate";

        // Act
        _mockEmailService.Object.SendActivationEmail(email, activationLink);

        // Assert
        _mockEmailService.Verify(service => service.SendActivationEmail(email, activationLink), Times.Once);
    }

    [Fact]
    public void SendPasswordResetEmail_ShouldSendEmail()
    {
        // Arrange
        var email = "test@example.com";
        var resetLink = "http://example.com/reset";

        // Act
        _mockEmailService.Object.SendPasswordResetEmail(email, resetLink);

        // Assert
        _mockEmailService.Verify(service => service.SendPasswordResetEmail(email, resetLink), Times.Once);
    }
} 