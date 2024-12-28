using SecureAuthLib.Business.Abstract;
using SecureAuthLib.DataAccess.Abstract;
using SecureAuthLib.Entities.Concrete;
using System;
using System.Security.Cryptography;
using System.Linq;

namespace SecureAuthLib.Business.Concrete
{
    public class UserManager : IUserService
    {
        private readonly IUserDal _userDal;
        private readonly IEmailService _emailService;

        public UserManager(IUserDal userDal, IEmailService emailService)
        {
            _userDal = userDal;
            _emailService = emailService;
        }

        // Retrieves a user by email
        public User GetUserByEmail(string email)
        {
            var user = _userDal.Get(u => u.Email == email);
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }
            return user;
        }

        // Authenticates a user with email and password
        public User Login(string email, string password)
        {
            var user = _userDal.Get(u => u.Email == email);
            if (user == null) {
                throw new InvalidOperationException("This email does not exist");
            }

            // Verify password
            if (!PasswordHelper.VerifyPassword(password, user.Salt, user.PasswordHash))
            {
                throw new InvalidOperationException("Incorrect password");
            }

            return user;
        }

        // Registers a new user
        public void Register(string email, string password)
        {
            var user = _userDal.Get(u => u.Email == email);
            if (user != null)
            {
                throw new InvalidOperationException("This email is already registered.");
            }

            // Hash the password
            string salt;
            string hashedPassword = PasswordHelper.HashPassword(password, out salt);

            // Create new user
            var newUser = new User
            {
                Email = email,
                PasswordHash = hashedPassword,
                Salt = salt,
                CreatedAt = DateTime.Now,
                IsActive = false // User is initially inactive
            };

            _userDal.Add(newUser);

            // Generate activation token and send email
            string activationToken = GenerateResetToken();
            newUser.ActivationToken = activationToken;
            _userDal.Update(newUser);

            string activationLink = $"https://yourapp.com/activate?token={activationToken}";
            _emailService.SendActivationEmail(email, activationLink);
        }

        // Requests a password reset for a user
        public void RequestPasswordReset(string email)
        {
            var user = GetUserByEmail(email);

            // Generate reset token
            string resetToken = GenerateResetToken();
            DateTime tokenExpiry = DateTime.UtcNow.AddHours(24);

            // Update user with reset token
            user.ResetToken = resetToken;
            user.ResetTokenExpiry = tokenExpiry;
            _userDal.Update(user);

            // Send password reset email
            string resetLink = $"https://yourapp.com/reset-password?token={resetToken}";
            _emailService.SendPasswordResetEmail(email, resetLink);
        }

        // Resets a user's password
        public void ResetPassword(string resetToken, string newPassword)
        {
            var user = _userDal.Get(u => u.ResetToken == resetToken);

            if (user == null || !IsTokenValid(user.ResetTokenExpiry))
            {
                throw new InvalidOperationException("Invalid or expired token");
            }

            // Hash the new password
            string salt;
            string hashedPassword = PasswordHelper.HashPassword(newPassword, out salt);

            // Update user with new password
            user.PasswordHash = hashedPassword;
            user.Salt = salt;
            user.ResetToken = null;
            user.ResetTokenExpiry = null;

            _userDal.Update(user);
        }

        // Checks if a token is valid
        internal bool IsTokenValid(DateTime? tokenExpiry)
        {
            return tokenExpiry.HasValue && tokenExpiry.Value > DateTime.UtcNow;
        }

        // Generates a secure reset token
        private string GenerateResetToken()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] randomBytes = new byte[64];
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        // Activates a user account
        public void ActivateUser(string activationToken)
        {
            var user = _userDal.Get(u => u.ActivationToken == activationToken);
            if (user == null)
            {
                throw new InvalidOperationException("Invalid activation token.");
            }

            user.IsActive = true;
            user.ActivationToken = null;
            _userDal.Update(user);
        }
    }
}
