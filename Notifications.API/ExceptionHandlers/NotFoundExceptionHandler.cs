using Microsoft.AspNetCore.Diagnostics;
using Notifications.API.Exceptions;

namespace Notifications.API.ExceptionHandlers
{
    public class NotFoundExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not NotFoundException ex) return false;
            
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new
            {
               type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
               title = "Not Found",
               status = 404,
               detail = ex.Message,
                instance = context.Request.Path.Value,
               errorCode = ex.ErrorCode,// Toma dinámicamente NTF-001 o NTF-003 dependiendo del caso
                errorMessage = ex.Message
            }, cancellationToken);

            return true;
        }
    }
}
