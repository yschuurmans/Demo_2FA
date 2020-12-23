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
    public class TotpService : ITotpService
    {
        private readonly Context context;
        private readonly TwoFactorAuth tfa;

        public TotpService(Context context)
        {
            this.context = context;
            tfa = new TwoFactorAuth("Demo 2fa");
        }

        public bool ValidateToken(string username, string token)
        {
            var account = context.Accounts.FirstOrDefault(x => x.Username == username);
            if (account == null || account.Secret == null) return false;

            long timeslice = 0;
            if(tfa.VerifyCode(account.Secret, token, out timeslice))
            {
                if(account.Timeslice < timeslice) {
                    account.Timeslice = timeslice;
                    context.SaveChanges();
                    return true;
                }
            }

            return false;
        }

        public string GenerateSecret()
        {
            var secret = tfa.CreateSecret(160);
            return secret;
        }

        public string GenerateQRCode(string username, string secret)
        {
            return tfa.GetQrCodeImageAsDataUri(username, secret);
        }

        public void SaveSecret(string username, string secret)
        {
            context.Accounts.First(x => x.Username == username).Secret = secret;
            context.SaveChanges();
        }

        public bool CheckToken(string secret, string token)
        {
            return tfa.VerifyCode(secret, token);
        }

        public bool HasTotpSetup(string username)
        {
            var account = context.Accounts.FirstOrDefault(x => x.Username == username);
            return account?.Secret != null;
        }
    }
}
