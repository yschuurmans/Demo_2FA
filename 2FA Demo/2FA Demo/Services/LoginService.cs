using _2FA_Demo.Database;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TwoFactorAuthNet;

namespace _2FA_Demo.Services
{
    public class LoginService : ILoginService
    {
        private readonly Context context;
        private readonly TwoFactorAuth tfa;

        public LoginService(Context context)
        {
            this.context = context;
            tfa = new TwoFactorAuth("Demo 2fa");
        }

        public bool ValidateCredentials(string username, string password)
        {
            string salt = context.Accounts.FirstOrDefault(x => x.Username == username)?.Salt;
            if (salt == null) return false;
            string hashedPassword = Hash(password, salt);

            return context.Accounts.Any(x=> x.Username == username && x.Password == hashedPassword);
        }

        public string RegisterAccount(string username, string password)
        {
            if (context.Accounts.Any(x => x.Username == username))
            {
                return "Already exists";
            }

            byte[] saltBytes = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            var salt = Convert.ToBase64String(saltBytes);

            string hashedPassword = Hash(password, salt);

            context.Accounts.Add(new Account
            {
                Username = username,
                Password = hashedPassword,
                Salt = salt
            });
            context.SaveChanges();
            return null;
        }

        /// <summary>
        /// https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-5.0
        /// </summary>
        private string Hash(string password, string salt)
        {
            // derive a 256-bit subkey (use HMACSHA256 with 10,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Convert.FromBase64String(salt),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            return hashed;
        }
    }
}
