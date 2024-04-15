using SmsIntegration.Database.Models;

namespace SmsIntegration.BLL.Interfaces
{
    public interface ISmsProvider
    {
        Task InitializeMessageProvider(SmsProvider provider);

        Task<(bool success, string message)> SendMessage(string phoneNumber, string messageText);

    }
}
