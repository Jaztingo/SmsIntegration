namespace SmsIntegration.Extensions.Consul.Models
{
    public class ConsulConfigurationSource : IConfigurationSource
    {
        public IEnumerable<Uri> ConsulUrls { get; }

        public string Path { get; }

        public string Token { get; }

        public ConsulConfigurationSource(IEnumerable<Uri> consulUrls, string path, string token)
        {
            ConsulUrls = consulUrls;
            Path = path;
            Token = token;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ConsulConfigurationProvider(ConsulUrls, Path, Token);
        }
    }
}
