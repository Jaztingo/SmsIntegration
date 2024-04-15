using SmsIntegration.DAL.IRepository;
using SmsIntegration.Database.Contexts;
using SmsIntegration.Database.Models;

namespace SmsIntegration.DAL.Repository
{
    public class SmsFlowRepository : RepositoryBase<SmsFlow>, ISmsFlowRepository
    {
        public SmsFlowRepository(SmsIntegrationContext context) : base(context)
        {
        }
    }
}
