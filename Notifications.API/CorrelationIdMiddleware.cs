using Microsoft.Extensions.Primitives;

namespace Notifications.API
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private const string CorrelationIdHeaderKey = "X-Correlation-Id";

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Si se envió un Correlation ID se usa y sino genera uno nuevo
            if (!context.Request.Headers.TryGetValue(CorrelationIdHeaderKey, out StringValues correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
            }

             //Se agrega a la respuesta HTTP
             context.Response.Headers.Append(CorrelationIdHeaderKey, correlationId);

             using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId.ToString()))
             {
               await _next(context);
             }
            
        }

    }

}

