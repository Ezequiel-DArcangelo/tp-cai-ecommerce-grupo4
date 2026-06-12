using Microsoft.AspNetCore.Diagnostics;
using Notifications.API.Exceptions;

namespace Notifications.API.ExceptionHandlers
{
    public class BusinessRuleExceptionHandler : IExceptionHandler
    {
        // Declaramos el logger
        private readonly ILogger<BusinessRuleExceptionHandler> _logger;

        // Lo inyectamos en el constructor
        public BusinessRuleExceptionHandler(ILogger<BusinessRuleExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not BusinessRuleException ex) return false;

            // Para mantener la trazabilidad, extraemos el CorrelationId
            var correlationId = context.Items["CorrelationId"]?.ToString() ?? "N/A";

            // Loggeamos como Warning
            _logger.LogWarning("Violación de regla de negocio {ErrorCode}: {Message}", ex.ErrorCode, ex.Message);

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                title = "Bad Request",
                status = 400,
                detail = ex.Message,
                instance = context.Request.Path.Value,
                errorCode = ex.ErrorCode, // Toma dinámicamente NTF-002
                errorMessage = ex.Message,
                correlationId // El identificador para rastrear el error 
            }, cancellationToken);

            return true;
        }
    }
}
