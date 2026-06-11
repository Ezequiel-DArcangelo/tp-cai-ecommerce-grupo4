using Microsoft.AspNetCore.Diagnostics;
using Cart.API.Exceptions;

namespace Cart.API.ExceptionHandlers
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
                int statusCode = 422;
                string title = "Unprocessable Entity";
                string detail = "No se puede procesar la solicitud.";
                string typeUrl = "https://tools.ietf.org/html/rfc4918#section-11.2";

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