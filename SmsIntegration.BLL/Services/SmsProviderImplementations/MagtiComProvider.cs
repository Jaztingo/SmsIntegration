using SmsIntegration.BLL.Helpers;
using SmsIntegration.BLL.Interfaces;
using SmsIntegration.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsIntegration.BLL.Services.SmsProviderImplementations
{
    internal class MagtiComProvider : ISmsProvider
    {
        private string host;
        private string userId;
        public Task InitializeMessageProvider(SmsProvider provider)
        {
            host = provider.SmsProviderConfigs.FirstOrDefault(el => el.ConfigKey == "Host")?.ConfigValue;
            userId = provider.SmsProviderConfigs.FirstOrDefault(el => el.ConfigKey == "UserId")?.ConfigValue;
            return Task.CompletedTask;
        }

        public async Task<(bool success, string message)> SendMessage(string phoneNumber, string messageText)
        {
            try
            {
                var url = $"{host}/pls/sms/phttp2sms.Process?src={userId}&dst=995{phoneNumber}&txt={messageText}";
                (bool success, string message) = await HttpHelper.SendSmsByWebAsync(url);
                return (success, message);
            }
            catch (Exception ex)
            {
                bool success = false;
                string message = ex.Message;
                return (success, message);
            }
        }
    }
}
