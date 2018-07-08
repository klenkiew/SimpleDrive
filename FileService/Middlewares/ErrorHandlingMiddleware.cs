using System;
using System.Net;
using System.Threading.Tasks;
using FileService.Exceptions;
using FileService.Serialization;
using Microsoft.AspNetCore.Http;

namespace FileService.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly JsonSerializer serializer;

        public ErrorHandlingMiddleware(RequestDelegate next, JsonSerializer serializer)
        {
            this.next = next;
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
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