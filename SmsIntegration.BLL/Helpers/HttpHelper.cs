using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsIntegration.BLL.Helpers
{
    internal class HttpHelper
    {
        public static async Task<(bool success, string message)> SendSmsByWebAsync(string url)
        {
            string response = "";

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var client = new HttpClient();

            var httpResponseMessage = await client.SendAsync(request);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                response = await httpResponseMessage.Content
                    .ReadAsStringAsync();
                if (response == "Y")
                    return (true, response);
            }
            return (false, response);
        }
    }
}
