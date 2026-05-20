using Microsoft.AspNetCore.Diagnostics;
using Users.API.Exceptions;

namespace Users.API.ExceptionHandlers
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
                context.Response.StatusCode = 404;

                var problemDetails = new
                {
                    type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    title = "Not Found",
                    status = 404,
                    detail = "El recurso solicitado no fue encontrado.",
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