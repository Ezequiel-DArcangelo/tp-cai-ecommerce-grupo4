using Microsoft.AspNetCore.Diagnostics;
using Cart.API.Exceptions;

namespace Cart.API.ExceptionHandlers
{
    public class ValidationExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext context,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is ValidationException validationException)
            {
                var correlationId = context.Items["X-Correlation-Id"]?.ToString() ?? string.Empty;

                int statusCode = 400;
                string title = "Bad Request";
                string detail = "Los datos del carrito son inválidos.";
                string typeUrl = "https://tools.ietf.org/html/rfc7231#section-6.5.1";

                context.Response.StatusCode = statusCode;
                var problemDetails = new
                {
                    type = typeUrl,
                    title = title,
                    status = statusCode,
                    detail = detail,
                    instance = context.Request.Path.Value,
                    errorCode = validationException.ErrorCode,
                    errorMessage = validationException.Message,
                    correlationId = correlationId
                };
                await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
                return true;
            }
            return false;
        }
    }
}