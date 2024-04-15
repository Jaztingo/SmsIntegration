using SmsIntegration.Extensions;
using SmsIntegration.Extensions.Consul;
using SmsIntegration.Extensions.Infrastructure;
using SmsIntegration.Middleware;

namespace SmsIntegration
{
    public class Program
    {
        public static IServiceProvider GeneralJobServiceProvider;
        public static void Main(string[] args)
        {
            var consulAddress = Environment.GetEnvironmentVariable("_ConsulAddress");
            var consulToken = Environment.GetEnvironmentVariable("_ConsulToken");
            var builder = WebApplication.CreateBuilder(args);


            builder.Configuration
                .AddConsul(consulAddress, "ServiceConfig", consulToken)
                .AddConsul(consulAddress, "ServiceConfig/test-sms-integration", consulToken);
            
            builder.AddDatabase();
            builder.AddServices();
            builder.AddRepositories();
            builder.AddLogging();

            JobExtension.AttachJobs();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            var app = builder.Build();
            GeneralJobServiceProvider = app.Services.CreateScope().ServiceProvider;
            
            app.UseMiddleware<LoggingMiddleware>();
            
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
