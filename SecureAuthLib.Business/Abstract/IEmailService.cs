using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureAuthLib.Business.Abstract
{
    public interface IEmailService
    {
        // Sends an activation email to the user
        void SendActivationEmail(string email, string activationLink);

        // Sends a password reset email to the user
        void SendPasswordResetEmail(string email, string resetLink);
    }
}
