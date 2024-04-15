using SmsIntegration.Database.Models;

namespace SmsIntegration.DAL.IRepository
{
    public interface ISmsProviderRepository : IRepositoryBase<SmsProvider>
    {
        Task<SmsProvider> SelecteSmsProvider();
    }
}
