using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using Microsoft.Extensions.Logging;
using Elasticsearch.Net;

namespace SmsIntegration.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<LoggingMiddleware> logger;
        public LoggingMiddleware(
            RequestDelegate next,
            ILogger<LoggingMiddleware> logger)
        {
            this.logger = logger;
            this.next = next;

        }

        public async Task Invoke(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;
            var queryData = context.Request.Query.Select(el => new { key = el.Key, Value = el.Value }).ToList();
            context.Request.EnableBuffering();
            string requestBodyAsText = string.Empty;
            string UserID = context.User.Identity.Name ?? string.Empty;
            requestBodyAsText = await new StreamReader(context.Request.Body).ReadToEndAsync();

            context.Request.Body.Position = 0;
            var requestheader = JsonConvert.SerializeObject(context.Request.Headers);

            try
            {
                var requestData = new
                {
                    query = context.Request.QueryString,
                    body = requestBodyAsText,
                    headers = requestheader
                };

                logger.LogInformation(JsonConvert.SerializeObject(requestData));

                var memoryBodyStream = new MemoryStream();
                context.Response.Body = memoryBodyStream;

                await next(context);

                var responceHeader = JsonConvert.SerializeObject(context.Response.Headers);

                memoryBodyStream.Seek(0, SeekOrigin.Begin);
                string responceBodyAsText = await new StreamReader(memoryBodyStream).ReadToEndAsync();
                memoryBodyStream.Seek(0, SeekOrigin.Begin);

                await memoryBodyStream.CopyToAsync(originalBodyStream);

                var responceData = new
                {
                    query = context.Request.QueryString,
                    body = responceBodyAsText,
                    headers = responceHeader
                };

                logger.LogInformation(JsonConvert.SerializeObject(responceData));
            }
            catch (Exception error)
            {
                logger.LogError(error?.Message);
            }
        }
    }
}
