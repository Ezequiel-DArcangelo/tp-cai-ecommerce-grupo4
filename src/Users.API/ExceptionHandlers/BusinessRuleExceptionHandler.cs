using Microsoft.AspNetCore.Diagnostics;
using Users.API.Exceptions;

namespace Users.API.ExceptionHandlers
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
                // Decidir el status code segun el codigo de error
                int statusCode = 409;
                string title = "Conflict";
                string detail = "Ya existe un recurso con esos datos.";
                string typeUrl = "https://tools.ietf.org/html/rfc7231#section-6.5.9";

                if (businessRuleException.ErrorCode == "USR-003")
                {
                    statusCode = 401;
                    title = "Unauthorized";
                    detail = "Las credenciales no son válidas.";
                    typeUrl = "https://tools.ietf.org/html/rfc7235#section-3.1";
                }
                else if (businessRuleException.ErrorCode == "USR-004" || businessRuleException.ErrorCode == "USR-005")
                {
                    statusCode = 403;
                    title = "Forbidden";
                    detail = "El acceso está prohibido.";
                    typeUrl = "https://tools.ietf.org/html/rfc7231#section-6.5.3";
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
                    errorMessage = businessRuleException.Message
                };

                await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
                return true;
            }

            return false;
        }
    }
}
