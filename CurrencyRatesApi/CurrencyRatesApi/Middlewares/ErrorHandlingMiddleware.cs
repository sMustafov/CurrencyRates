using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using CurrencyRates.Entities.ErorrModel;
using CurrencyRates.Services.Utils.Exceptions;

namespace CurrencyRates.WebAPI.Middlewares
{
    /// <summary>
    /// Error handling middleware class
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        /// <summary>
        /// Request delegate
        /// </summary>
        private readonly RequestDelegate next;

        /// <summary>
        /// Inializes the new class
        /// </summary>
        /// <param name="next">The request delegate</param>
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>
        /// Invokes the Middleware
        /// </summary>
        /// <param name="context">The HTTP context</param>
        /// <param name="logger">The logger</param>
        /// <returns></returns>
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

        /// <summary>
        /// Handling exeption 
        /// </summary>
        /// <param name="context">The HTTP context</param>
        /// <param name="e">The exception</param>
        /// <returns></returns>
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
                resultMessage = JsonConvert.SerializeObject(new ErrorMessage { Message = "Internal Server Error", StatusCode = code });
            }

            return context.Response.WriteAsync(resultMessage);
        }
    }
}
