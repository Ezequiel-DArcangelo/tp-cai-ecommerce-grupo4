using Microsoft.AspNetCore.Diagnostics;
using Notifications.API.Exceptions;

namespace Notifications.API.ExceptionHandlers
{
    public class NotFoundExceptionHandler : IExceptionHandler
    {
        // Declaramos el logger
        private readonly ILogger<NotFoundExceptionHandler> _logger;

        // Lo inyectamos en el constructor
        public NotFoundExceptionHandler(ILogger<NotFoundExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            // Verificamos la excepción
            if (exception is not NotFoundException ex) return false;

            // Extraemos el CorrelationId 
            var correlationId = context.Items["CorrelationId"]?.ToString() ?? "N/A";

            // Loggeamos como Warning
            _logger.LogWarning("Recurso no encontrado {ErrorCode}: {Message}", ex.ErrorCode, ex.Message);

            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new
            {
               type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
               title = "Not Found",
               status = 404,
               detail = "El recurso solicitado no fue encontrado.",
               instance = context.Request.Path.Value,
               errorCode = ex.ErrorCode,// Toma dinámicamente NTF-001 o NTF-003 dependiendo del caso
               errorMessage = ex.Message,
               correlationId // El identificador para rastrear el error 
            }, cancellationToken);

            return true; // Manejamos el error 
        }
    }
}
