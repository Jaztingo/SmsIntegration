using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsIntegration.BLL.Enums
{
    public enum SmsFlowStatus
    {
        Created = 0,
        Started = 10,
        Sended = 20,
        Error = 30
    }
}
