using SmsIntegration.BLL.Interfaces;
using SmsIntegration.BLL.Services;
using SmsIntegration.DAL.IRepository;
using SmsIntegration.DAL.Repository;

namespace SmsIntegration.Extensions.Infrastructure
{
    public static class ServiceExtensions
    {
        public static void AddServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<ISmsServices, SmsServices>();
        }
        public static void AddRepositories(this WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<ISmsFlowRepository, SmsFlowRepository>();
            builder.Services.AddTransient<ISmsProviderRepository, SmsProviderRepository>();
            builder.Services.AddTransient<ISmsRepository, SmsRepository>();
        }
    }
}
