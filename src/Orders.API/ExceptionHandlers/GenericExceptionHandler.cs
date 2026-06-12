using Microsoft.AspNetCore.Diagnostics;
using Orders.API.Exceptions;

namespace Orders.API.ExceptionHandlers

/// <summary>
/// Manejador genérico de excepciones para errores internos en Orders.API.
/// Devuelve un JSON con los detalles del error y el código corporativo ORD-007.
/// </summary>
{
    public class GenericExceptionHandler : IExceptionHandler

    {
        /// <summary>
        /// Intenta manejar cualquier excepción no controlada devolviendo un JSON con detalles del error.
        /// </summary>
        /// <param name="context">Contexto HTTP actual.</param>
        /// <param name="exception">Excepción capturada.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <returns>True si la excepción fue manejada.</returns>
        public async ValueTask<bool> TryHandleAsync(
            HttpContext context,
            Exception exception,
            CancellationToken cancellationToken)
        {
            int statusCode = 500;
            string title = "Internal Server Error";
            string detail = "Error interno al procesar la orden.";
            string typeUrl = "https://tools.ietf.org/html/rfc7231#section-6.6.1";

            var correlationId = context.Items["X-Correlation-Id"]?.ToString() ?? string.Empty;

            context.Response.StatusCode = statusCode;

            var problemDetails = new
            {
                type = typeUrl,
                title = title,
                status = statusCode,
                detail = detail,
                instance = context.Request.Path.Value,
                errorCode = "ORD-007",
                errorMessage = exception.Message,
                correlationId = correlationId
            };

            await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }
    }
}