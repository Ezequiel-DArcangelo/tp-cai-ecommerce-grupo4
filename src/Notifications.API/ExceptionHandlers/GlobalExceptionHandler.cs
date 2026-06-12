using Microsoft.AspNetCore.Diagnostics;

namespace Notifications.API.ExceptionHandlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        // Declaramos el logger
        private readonly ILogger<GlobalExceptionHandler> _logger;

        // Lo inyectamos en el constructor
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        

        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            // Este manejador no lleva IF porque se encarga de manejar cualquier excepción no manejada por los otros handlers.

            // Extraemos el CorrelationId
            var correlationId = context.Items["CorrelationId"]?.ToString() ?? "N/A";

            // Loggeamos como Error
            _logger.LogError(exception, "Error inesperado en el servidor: {Message}", exception.Message);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(new
            {
                type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                title = "Internal Server Error",
                status = 500,
                detail = "Ocurrió un error inesperado en el servidor.",
                instance = context.Request.Path.Value,
                errorCode = "NTF-004", // Código genérico para errores no manejados
                errorMessage = exception.Message,
                correlationId // Incluimos el ID para saber que request causó el error
            }, cancellationToken);

            return true;
        }
    }
}
