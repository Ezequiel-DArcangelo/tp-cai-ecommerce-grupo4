using Microsoft.AspNetCore.Diagnostics;

namespace Cart.API.ExceptionHandlers
{
    public class GenericExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext context,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var correlationId = context.Items["X-Correlation-Id"]?.ToString() ?? string.Empty;
            int statusCode = 500;
            string title = "Internal Server Error";
            string detail = "Error interno al procesar el carrito.";
            string typeUrl = "https://tools.ietf.org/html/rfc7231#section-6.6.1";

            context.Response.StatusCode = statusCode;
            var problemDetails = new
            {
                type = typeUrl,
                title = title,
                status = statusCode,
                detail = detail,
                instance = context.Request.Path.Value,
                errorCode = "CRT-005",
                errorMessage = exception.Message,
                correlationId = correlationId
            };
            await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }
    }
}