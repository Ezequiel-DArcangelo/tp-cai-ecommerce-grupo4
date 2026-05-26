using Microsoft.AspNetCore.Diagnostics;
using Orders.API.Exceptions;

namespace Orders.API.ExceptionHandlers
{
    public class GenericExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext context,
            Exception exception,
            CancellationToken cancellationToken)
        {
            int statusCode = 500;
            string title = "Internal Server Error";
            string detail = "Error interno al procesar la orden.";
            string typeUrl = "https://tools.ietf.org/html/rfc7231#section-6.6.1";

            context.Response.StatusCode = statusCode;

            var problemDetails = new
            {
                type = typeUrl,
                title = title,
                status = statusCode,
                detail = detail,
                instance = context.Request.Path.Value,
                errorCode = "ORD-007",
                errorMessage = exception.Message
            };

            await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }
    }
}