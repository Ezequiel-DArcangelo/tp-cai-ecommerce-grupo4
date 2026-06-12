using Microsoft.Extensions.Primitives;
using Serilog.Context;

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
            // Intentamos leerlo del request si se envió
            if (!context.Request.Headers.TryGetValue(CorrelationIdHeaderKey, out var correlationId))
            {
                // sino geneeramos uno nuevo
                correlationId = Guid.NewGuid().ToString();
            }

            // Lo guardamos en Items (parque lo lean los handlers)
            context.Items["CorrelationId"] = correlationId.ToString();

            //Se agrega a la respuesta HTTP
            context.Response.Headers.Append(CorrelationIdHeaderKey, correlationId.ToString());

             using (LogContext.PushProperty("CorrelationId", correlationId.ToString()))
             {
               await _next(context);
             }
            
        }

    }

}

