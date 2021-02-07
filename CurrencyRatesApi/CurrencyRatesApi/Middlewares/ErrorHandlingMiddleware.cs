using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using CurrencyRatesApi.Utils;
using CurrencyRates.Services.Utils.Exceptions;

namespace CurrencyRatesApi.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }
        public async Task Invoke(HttpContext context, ILogger<ErrorHandlingMiddleware> logger)
        {
            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                logger.LogWarning(e.Message);

                await HandleExceptionAsync(context, e);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception e)
        {
            var code = StatusCodes.Status500InternalServerError;

            if (e is CustomInvalidOperationException)
            {
                code = StatusCodes.Status400BadRequest;
            }
            else if (e is CustomArgumentException)
            {
                code = StatusCodes.Status404NotFound;
            }

            var resultMessage = JsonConvert.SerializeObject(new ErrorMessage { Message = e.Message, StatusCode = code });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = code;

            if (code == StatusCodes.Status500InternalServerError)
            {
                resultMessage = JsonConvert.SerializeObject(new ErrorMessage { Message = "Internal Server Error!", StatusCode = code });
            }

            return context.Response.WriteAsync(resultMessage);
        }
    }
}
