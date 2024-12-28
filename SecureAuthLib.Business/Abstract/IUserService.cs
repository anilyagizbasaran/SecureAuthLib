using SecureAuthLib.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureAuthLib.Business.Abstract
{
    public interface IUserService
    {
        void Register(string email, string password);
        User Login(string email, string password);
        void ResetPassword(string email, string newPassword);
        User GetUserByEmail(string email);
    }
}
