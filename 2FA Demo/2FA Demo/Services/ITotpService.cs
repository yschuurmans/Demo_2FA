namespace _2FA_Demo.Services
{
    public interface ITotpService
    {
        bool ValidateToken(string username, string token);
        string GenerateSecret();
        string GenerateQRCode(string username, string secret);
        void SaveSecret(string username, string secret);
        bool CheckToken(string secret, string token);
        bool HasTotpSetup(string username);
    }
}