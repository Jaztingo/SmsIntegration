using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsIntegration.BLL.Interfaces
{
    public interface ISmsServices
    {
        Task<long> CreateMessage(string phoneNumber, string messageText);
        Task SendMessage(long messageId);
    }
}
