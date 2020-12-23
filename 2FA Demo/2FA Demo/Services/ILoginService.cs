namespace _2FA_Demo.Services
{
    public interface ILoginService
    {
        string RegisterAccount(string username, string password);
        bool ValidateCredentials(string username, string password);
    }
}