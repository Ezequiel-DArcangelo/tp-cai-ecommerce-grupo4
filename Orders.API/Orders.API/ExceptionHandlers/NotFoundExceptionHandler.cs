using Microsoft.AspNetCore.Diagnostics;
using Orders.API.Exceptions;

namespace Orders.API.ExceptionHandlers
{
    public class NotFoundExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext context,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is NotFoundException notFoundException)
            {
                int statusCode = 404;
                string title = "Not Found";
                string detail = "El recurso solicitado no existe.";
                string typeUrl = "https://tools.ietf.org/html/rfc7231#section-6.5.4";

                context.Response.StatusCode = statusCode;

                var problemDetails = new
                {
                    type = typeUrl,
                    title = title,
                    status = statusCode,
                    detail = detail,
                    instance = context.Request.Path.Value,
                    errorCode = notFoundException.ErrorCode,
                    errorMessage = notFoundException.Message
                };

                await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
                return true;
            }

            return false;
        }
    }
}