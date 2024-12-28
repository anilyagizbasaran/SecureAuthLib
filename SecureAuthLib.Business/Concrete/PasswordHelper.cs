using System;
using System.Security.Cryptography;
using System.Text;

public static class PasswordHelper
{
   
    public static string HashPassword(string password, out string salt)
    {
        
        byte[] saltBytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        salt = Convert.ToBase64String(saltBytes);

        
        using (var sha256 = SHA256.Create())
        {
            string saltedPassword = salt + password;
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
            return Convert.ToBase64String(hashBytes);
        }
    }

    
    public static bool VerifyPassword(string password, string salt, string hash)
    {
        using (var sha256 = SHA256.Create())
        {
            string saltedPassword = salt + password;
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
            string newHash = Convert.ToBase64String(hashBytes);
            return newHash == hash;
        }
    }
}
