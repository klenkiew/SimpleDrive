using System;
using System.Net;
using System.Threading.Tasks;
using FileService.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serialization;

namespace FileService.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly JsonSerializer serializer;
        private readonly ILogger logger;

        public ErrorHandlingMiddleware(RequestDelegate next, JsonSerializer serializer, ILoggerFactory loggerFactory)
        {
            this.next = next;
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            this.logger = loggerFactory.CreateLogger(typeof(ErrorHandlingMiddleware));
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            logger.LogError(exception, "An error occured during processing a request in the pipeline.");
            
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected

            switch (exception)
            {
                case NotFoundException _:
                    code = HttpStatusCode.NotFound;
                    break;
                case PermissionException _:
                    code = HttpStatusCode.Forbidden;
                    break;
            }

            var result = serializer.Serialize(new { exception.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}