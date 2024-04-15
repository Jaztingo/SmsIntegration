using SmsIntegration.DAL.IRepository;
using SmsIntegration.Database.Contexts;
using SmsIntegration.Database.Models;

namespace SmsIntegration.DAL.Repository
{
    public class SmsRepository : RepositoryBase<Smss>, ISmsRepository
    {
        public SmsRepository(SmsIntegrationContext context) : base(context)
        {
        }
    }
}
