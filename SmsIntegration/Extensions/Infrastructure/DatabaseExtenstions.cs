using Microsoft.EntityFrameworkCore;
using SmsIntegration.Database.Contexts;

namespace SmsIntegration.Extensions.Infrastructure
{
    public static class DatabaseExtenstions
    {
        public static void AddDatabase(this WebApplicationBuilder builder)
        {
            var dbConnectionString = builder.Configuration["ConnectionString"];
            builder
                .Services
                .AddDbContext<SmsIntegrationContext>(options => options.UseSqlServer(dbConnectionString));


        }
    }
}
