using SecureAuthLib.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureAuthLib.Entities.Concrete
{
    public class User : IEntity
    {
        public int UserId { get; set; } // Unique identifier for the user
        public string Email { get; set; } // User's email address
        public string PasswordHash { get; set; } // Hashed password
        public string Salt { get; set; } // Salt used for hashing the password
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Account creation date
        public bool IsActive { get; set; } // Indicates if the account is active
        public string ActivationToken { get; set; } // Token for account activation
        public string ResetToken { get; set; } // Token for password reset
        public DateTime? ResetTokenExpiry { get; set; } // Expiry date for the reset token
    }
}
