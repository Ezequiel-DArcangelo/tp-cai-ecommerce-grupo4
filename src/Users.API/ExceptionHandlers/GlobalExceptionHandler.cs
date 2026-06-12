using Microsoft.AspNetCore.Diagnostics;

namespace Users.API.ExceptionHandlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext context,
            Exception exception,
            CancellationToken cancellationToken)
        {
            // Este handler atrapa cualquier excepcion no manejada por los otros.
            // Devuelve 500 con USR-006 como red de seguridad.

            context.Response.StatusCode = 500;

            var problemDetails = new
            {
                type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                title = "Internal Server Error",
                status = 500,
                detail = "Ocurrió un error inesperado al procesar la solicitud.",
                instance = context.Request.Path.Value,
                errorCode = "USR-006",
                errorMessage = "Error interno al procesar el usuario."
            };

            await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }
    }
}