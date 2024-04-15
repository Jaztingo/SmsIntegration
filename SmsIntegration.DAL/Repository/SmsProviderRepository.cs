using Microsoft.EntityFrameworkCore;
using SmsIntegration.DAL.IRepository;
using SmsIntegration.Database.Contexts;
using SmsIntegration.Database.Models;
using System;
using System.Linq.Expressions;

namespace SmsIntegration.DAL.Repository
{
    public class SmsProviderRepository : RepositoryBase<SmsProvider>, ISmsProviderRepository
    {
        public SmsProviderRepository(SmsIntegrationContext context) : base(context)
        {
        }
        public async Task<SmsProvider> SelecteSmsProvider()
        {
            int perCent = (byte)new Random().Next(0, 100);
            var selectedSmsProviders = await base
                .GetAsync(el => el.Active && el.ChanceOfSelection <= perCent, extra => extra.SmsProviderConfigs);
            return selectedSmsProviders?.FirstOrDefault();
        }
    }
}
