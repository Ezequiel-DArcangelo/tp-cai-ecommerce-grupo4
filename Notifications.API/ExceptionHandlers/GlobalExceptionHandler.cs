using Microsoft.AspNetCore.Diagnostics;

namespace Notifications.API.ExceptionHandlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            // Este manejador no lleva IF porque se encarga de manejar cualquier excepción no manejada por los otros handlers.
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new
            {
                type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                title = "Internal Server Error",
                status = 500,
                detail = "Ocurrió un error inesperado en el servidor.",
                instance = context.Request.Path.Value,
                errorCode = "NTF-004", // Código genérico para errores no manejados
                errorMessage = exception.Message
            }, cancellationToken);

            return true;
        }
    }
}
