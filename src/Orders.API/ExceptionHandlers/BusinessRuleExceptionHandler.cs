using Microsoft.AspNetCore.Diagnostics;
using Orders.API.Exceptions;

namespace Orders.API.ExceptionHandlers
{
    public class BusinessRuleExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext context,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is BusinessRuleException businessRuleException)
            { 
                var correlationId = context.Items["X-Correlation-Id"]?.ToString() ?? string.Empty;
            
                // Valores por defecto
                int statusCode = 422;
                string title = "Unprocessable Entity";
                string detail = "No se puede procesar la solicitud.";
                string typeUrl = "https://tools.ietf.org/html/rfc4918#section-11.2";


                // Caso específico: transición inválida de estado (ORD-006)
                if (businessRuleException.ErrorCode == "ORD-006")
                {
                    statusCode = 409;
                    title = "Conflict";
                    detail = "El estado de la orden no puede ser modificado.";
                    typeUrl = "https://tools.ietf.org/html/rfc7231#section-6.5.8";
                }

                // Caso específico: stock insuficiente (ORD-005)
                else if (businessRuleException.ErrorCode == "ORD-005")
                {
                    statusCode = 422;
                    title = "Unprocessable Entity";
                    detail = "No se puede procesar la solicitud.";
                    typeUrl = "https://tools.ietf.org/html/rfc4918#section-11.2";
                }

                context.Response.StatusCode = statusCode;

                var problemDetails = new
                {
                    type = typeUrl,
                    title = title,
                    status = statusCode,
                    detail = detail,
                    instance = context.Request.Path.Value,
                    errorCode = businessRuleException.ErrorCode,
                    errorMessage = businessRuleException.Message,
                    correlationId = correlationId
                };

                await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
                return true;
            }

            return false;
        }
    }
}