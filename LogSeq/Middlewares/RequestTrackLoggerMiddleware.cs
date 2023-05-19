using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace LogSeq.Middlewares
{
    public class RequestTrackLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestTrackLoggerMiddleware> logger;

        public RequestTrackLoggerMiddleware(RequestDelegate next, ILogger<RequestTrackLoggerMiddleware> logger)
        {
            _next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            Dictionary<string, string> requestScope = new Dictionary<string, string>();

            requestScope["RequestId"] = Guid.NewGuid().ToString();
            requestScope["userId"] = System.Environment.MachineName;

            using var scope = logger.BeginScope(requestScope);
            await _next(httpContext);

            stopwatch.Stop();
            var sec = stopwatch.ElapsedMilliseconds / 1000.0;

            logger.LogInformation("request from {path}. start time {datetime}. period of time : {time} ",
                httpContext.Request.Path,
                DateTime.Now,
                sec);
        }
    }
}
