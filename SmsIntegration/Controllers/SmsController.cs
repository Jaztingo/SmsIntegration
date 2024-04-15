using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmsIntegration.BLL.Interfaces;
using SmsIntegration.Dto;

namespace SmsIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmsController : ControllerBase
    {
        private readonly ISmsServices smsServices;
        public SmsController(ISmsServices smsServices)
        {
            this.smsServices = smsServices;
        }

        [HttpPost]
        public async Task<IActionResult> SendSms(MessageDto model)
        {
            long messageId = await smsServices.CreateMessage(model.phoneNumber, model.messageText);
            await smsServices.SendMessage(messageId);
            return Ok(new
            {
                status = 0,
                desc = "Request received"
            });
        }
    }
}
