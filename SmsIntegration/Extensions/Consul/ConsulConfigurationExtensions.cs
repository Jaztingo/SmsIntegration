
using SmsIntegration.Extensions.Consul.Models;

namespace SmsIntegration.Extensions.Consul
{
    public static class ConsulConfigurationExtensions
    {
        public static IConfigurationBuilder AddConsul(this IConfigurationBuilder configurationBuilder, IEnumerable<Uri> consulUrls, string consulPath, string token)
        {
            return configurationBuilder.Add(new ConsulConfigurationSource(consulUrls, consulPath, token));
        }
        public static IConfigurationBuilder AddConsul(this IConfigurationBuilder configurationBuilder, IEnumerable<string> consulUrls, string consulPath, string token)
        {
            return configurationBuilder.AddConsul(consulUrls.Select((u) => new Uri(u)), consulPath, token);
        }

        public static IConfigurationBuilder AddConsul(this IConfigurationBuilder configurationBuilder, string consulUrl, string consulPath, string token)
        {
            return configurationBuilder.AddConsul(new Uri[1] { new Uri(consulUrl) }, consulPath, token);
        }
    }
}
