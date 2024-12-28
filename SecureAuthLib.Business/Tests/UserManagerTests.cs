using Moq;
using SecureAuthLib.Business.Abstract;
using SecureAuthLib.Business.Concrete;
using SecureAuthLib.DataAccess.Abstract;
using SecureAuthLib.Entities.Concrete;
using System;
using System.Linq.Expressions;
using Xunit;

public class UserManagerTests
{
    private readonly Mock<IUserDal> _mockUserDal;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly UserManager _userManager;

    public UserManagerTests()
    {
        _mockUserDal = new Mock<IUserDal>();
        _mockEmailService = new Mock<IEmailService>();
        _userManager = new UserManager(_mockUserDal.Object, _mockEmailService.Object);
    }

    [Fact]
    public void Register_ShouldThrowException_WhenEmailAlreadyExists()
    {
        // Arrange
        var email = "test@example.com";
        _mockUserDal.Setup(dal => dal.Get(It.IsAny<Expression<Func<User, bool>>>()))
                    .Returns(new User { Email = email });

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _userManager.Register(email, "password123"));
    }

    [Fact]
    public void Register_ShouldSendActivationEmail_WhenUserIsRegistered()
    {
        // Arrange
        var email = "newuser@example.com";
        _mockUserDal.Setup(dal => dal.Get(It.IsAny<Expression<Func<User, bool>>>()))
                    .Returns((User)null);

        // Act
        _userManager.Register(email, "password123");

        // Assert
        _mockEmailService.Verify(service => service.SendActivationEmail(email, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void ActivateUser_ShouldActivateUser_WhenTokenIsValid()
    {
        // Arrange
        var activationToken = "valid-token";
        var user = new User { ActivationToken = activationToken, IsActive = false };
        _mockUserDal.Setup(dal => dal.Get(It.IsAny<Expression<Func<User, bool>>>()))
                    .Returns(user);

        // Act
        _userManager.ActivateUser(activationToken);

        // Assert
        Assert.True(user.IsActive);
        Assert.Null(user.ActivationToken);
        _mockUserDal.Verify(dal => dal.Update(user), Times.Once);
    }

    [Fact]
    public void Login_ShouldReturnUser_WhenCredentialsAreValid()
    {
        // Arrange
        var email = "validuser@example.com";
        var password = "validpassword";
        var salt = "randomsalt";
        var hashedPassword = PasswordHelper.HashPassword(password, out salt);

        var user = new User { Email = email, PasswordHash = hashedPassword, Salt = salt };
        _mockUserDal.Setup(dal => dal.Get(It.IsAny<Expression<Func<User, bool>>>()))
                    .Returns(user);

        // Act
        var result = _userManager.Login(email, password);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
    }

    [Fact]
    public void Login_ShouldThrowException_WhenEmailDoesNotExist()
    {
        // Arrange
        var email = "nonexistent@example.com";
        _mockUserDal.Setup(dal => dal.Get(It.IsAny<Expression<Func<User, bool>>>()))
                    .Returns((User)null);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _userManager.Login(email, "password"));
    }

    [Fact]
    public void Login_ShouldThrowException_WhenPasswordIsIncorrect()
    {
        // Arrange
        var email = "validuser@example.com";
        var password = "validpassword";
        var salt = "randomsalt";
        var hashedPassword = PasswordHelper.HashPassword(password, out salt);

        var user = new User { Email = email, PasswordHash = hashedPassword, Salt = salt };
        _mockUserDal.Setup(dal => dal.Get(It.IsAny<Expression<Func<User, bool>>>()))
                    .Returns(user);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _userManager.Login(email, "wrongpassword"));
    }

    [Fact]
    public void RequestPasswordReset_ShouldAssignResetTokenAndSendEmail_WhenEmailExists()
    {
        // Arrange
        var email = "existinguser@example.com";
        var user = new User { Email = email };
        _mockUserDal.Setup(dal => dal.Get(It.IsAny<Expression<Func<User, bool>>>()))
                    .Returns(user);

        // Act
        _userManager.RequestPasswordReset(email);

        // Assert
        Assert.NotNull(user.ResetToken);
        Assert.True(user.ResetTokenExpiry > DateTime.UtcNow);
        _mockUserDal.Verify(dal => dal.Update(user), Times.Once);
        _mockEmailService.Verify(service => service.SendPasswordResetEmail(email, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void RequestPasswordReset_ShouldThrowException_WhenEmailDoesNotExist()
    {
        // Arrange
        var email = "nonexistent@example.com";
        _mockUserDal.Setup(dal => dal.Get(It.IsAny<Expression<Func<User, bool>>>()))
                    .Returns((User)null);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _userManager.RequestPasswordReset(email));
    }

    [Fact]
    public void ResetPassword_ShouldUpdatePasswordAndClearToken_WhenTokenIsValid()
    {
        // Arrange
        var resetToken = "valid-token";
        var newPassword = "newpassword";
        var user = new User { ResetToken = resetToken, ResetTokenExpiry = DateTime.UtcNow.AddHours(1) };
        _mockUserDal.Setup(dal => dal.Get(It.IsAny<Expression<Func<User, bool>>>()))
                    .Returns(user);

        // Act
        _userManager.ResetPassword(resetToken, newPassword);

        // Assert
        Assert.NotNull(user.PasswordHash);
        Assert.Null(user.ResetToken);
        Assert.Null(user.ResetTokenExpiry);
        _mockUserDal.Verify(dal => dal.Update(user), Times.Once);
    }

    [Fact]
    public void ResetPassword_ShouldThrowException_WhenTokenIsInvalidOrExpired()
    {
        // Arrange
        var resetToken = "invalid-token";
        _mockUserDal.Setup(dal => dal.Get(It.IsAny<Expression<Func<User, bool>>>()))
                    .Returns((User)null);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _userManager.ResetPassword(resetToken, "newpassword"));
    }

    [Fact]
    public void IsTokenValid_ShouldReturnTrue_WhenTokenIsNotExpired()
    {
        // Arrange
        var tokenExpiry = DateTime.UtcNow.AddHours(1);

        // Act
        var result = _userManager.IsTokenValid(tokenExpiry);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsTokenValid_ShouldReturnFalse_WhenTokenIsExpired()
    {
        // Arrange
        var tokenExpiry = DateTime.UtcNow.AddHours(-1);

        // Act
        var result = _userManager.IsTokenValid(tokenExpiry);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ActivateUser_ShouldThrowException_WhenTokenIsInvalid()
    {
        // Arrange
        var activationToken = "invalid-token";
        _mockUserDal.Setup(dal => dal.Get(It.IsAny<Expression<Func<User, bool>>>()))
                    .Returns((User)null);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _userManager.ActivateUser(activationToken));
    }
}