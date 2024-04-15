using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;

namespace SmsIntegration.Extensions.Consul.Models
{
    public class ConsulConfigurationProvider : ConfigurationProvider
    {
        private const string ConsulIndexHeader = "X-Consul-Index";

        private readonly string _path;

        private readonly HttpClient _httpClient;

        private readonly IReadOnlyList<Uri> _consulUrls;

        private readonly Task _configurationListeningTask;

        private int _consulUrlIndex;

        private int _failureCount;

        private int _consulConfigurationIndex;

        private readonly string _token;

        public ConsulConfigurationProvider(IEnumerable<Uri> consulUrls, string path, string token)
        {
            _path = path;
            _token = token;
            _consulUrls = consulUrls.Select((u) => new Uri(u, "v1/kv/" + path)).ToList();
            if (_consulUrls.Count <= 0)
            {
                throw new ArgumentOutOfRangeException("consulUrls");
            }

            _httpClient = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            }, disposeHandler: true);
            _httpClient.DefaultRequestHeaders.Add("X-Consul-Token", token);
            _configurationListeningTask = new Task(ListenToConfigurationChanges);
        }

        public override void Load()
        {
            LoadAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter().GetResult();
        }

        private async Task LoadAsync()
        {
            Data = await ExecuteQueryAsync();
            if (_configurationListeningTask.Status == TaskStatus.Created)
            {
                _configurationListeningTask.Start();
            }
        }

        private async void ListenToConfigurationChanges()
        {
            _ = 1;
            while (true)
            {
                try
                {
                    if (_failureCount > _consulUrls.Count)
                    {
                        _failureCount = 0;
                        await Task.Delay(TimeSpan.FromMinutes(1.0));
                    }

                    Data = await ExecuteQueryAsync(isBlocking: true);
                    OnReload();
                    _failureCount = 0;
                }
                catch (TaskCanceledException)
                {
                    _failureCount = 0;
                }
                catch
                {
                    _consulUrlIndex = (_consulUrlIndex + 1) % _consulUrls.Count;
                    _failureCount++;
                }
            }
        }

        private async Task<IDictionary<string, string>> ExecuteQueryAsync(bool isBlocking = false)
        {
            string relativeUri = isBlocking ? $"?recurse=true&index={_consulConfigurationIndex}" : "?recurse=true";
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri(_consulUrls[_consulUrlIndex], relativeUri));
            using HttpResponseMessage response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            if (response.Headers.Contains("X-Consul-Index"))
            {
                int.TryParse(response.Headers.GetValues("X-Consul-Index").FirstOrDefault(), out _consulConfigurationIndex);
            }

            return (from k in JToken.Parse(await response.Content.ReadAsStringAsync())
                    select new KeyValuePair<string, string>(k.Value<string>("Key").Substring(_path.Length + 1), k.Value<string>("Value") != null ? Encoding.UTF8.GetString(Convert.FromBase64String(k.Value<string>("Value"))) : null) into v
                    where !string.IsNullOrWhiteSpace(v.Key)
                    select v).ToDictionary((v) => ConfigurationPath.Combine(v.Key.Split('/')), (v) => v.Value, StringComparer.OrdinalIgnoreCase);
        }

        private static IEnumerable<KeyValuePair<string, string>> Flatten(KeyValuePair<string, JToken> tuple)
        {
            if (!(tuple.Value is JObject jObject))
            {
                yield break;
            }

            foreach (KeyValuePair<string, JToken> item in jObject)
            {
                string key = tuple.Key + "/" + item.Key;
                switch (item.Value.Type)
                {
                    case JTokenType.Object:
                        foreach (KeyValuePair<string, string> item2 in Flatten(new KeyValuePair<string, JToken>(key, item.Value)))
                        {
                            yield return item2;
                        }

                        break;
                    default:
                        yield return new KeyValuePair<string, string>(key, item.Value.Value<string>());
                        break;
                    case JTokenType.Array:
                        break;
                }
            }
        }
    }
}
