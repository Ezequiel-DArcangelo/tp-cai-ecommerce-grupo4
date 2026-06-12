using Serilog.Context;

namespace Users.API.Middlewares
{
    // Middleware que asigna un Correlation ID unico a cada request HTTP.
    // Si el request viene con el header X-Correlation-Id, lo usa. Si no, genera uno.
    // Lo agrega al contexto de Serilog para que aparezca en todos los logs del request.
    public class CorrelationIdMiddleware
    {
        private const string HeaderName = "X-Correlation-Id";

        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Buscar si el request trae un X-Correlation-Id en los headers
            string correlationId;
            if (context.Request.Headers.ContainsKey(HeaderName) == true)
            {
                correlationId = context.Request.Headers[HeaderName].ToString();
            }
            else
            {
                // Si no vino, generar uno nuevo
                correlationId = Guid.NewGuid().ToString();
            }

            // Agregar el header en la respuesta para que el cliente lo pueda ver
            context.Response.Headers[HeaderName] = correlationId;

            // Agregar el ID al contexto de Serilog. A partir de aqui, todos los logs
            // del request van a incluir esta propiedad automaticamente.
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                // Pasar el control al siguiente middleware de la cadena
                await _next(context);
            }
        }
    }
}