using Microsoft.Extensions.DependencyInjection;
using Products.API.ExceptionHandlers;
using Products.API.Services;
using Serilog;
using Products.API;

var builder = WebApplication.CreateBuilder(args);

// Configuración del logging con Serilog
builder.AddAppLogging();

// Agregamos servicios al contenedor
builder.Services.AddControllers().ConfigureApiBehaviorOptions (options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Bad Request",
            Status = StatusCodes.Status400BadRequest,
            Detail = "Los datos proporcionados no son válidos o tienen un formato incorrecto.",
        };

        problemDetails.Extensions["errorCode"] = "PRD-002";
        problemDetails.Extensions["errorMessage"] = "Los datos del producto son inválidos.";

        return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(problemDetails);
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ProductService>(); // Registro del servicio de productos como Scoped
builder.Services.AddSingleton<DatabaseInitializer>(); // Registro del inicializador de la base de datos
builder.Services.AddScoped<ProductRepository>(); // Registro del repositorio de productos como Scoped
builder.Services.AddHttpClient(); // Para habilitar las llamadas HTTP desde el servicio de productos

// Manejadores de excepciones 
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<BusinessRuleExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    initializer.Initialize();
}

app.UseSerilogRequestLogging(options =>
{
    options.GetLevel = (httpContext, _, ex) =>
    (ex != null) ? Serilog.Events.LogEventLevel.Error :
    (httpContext.Request.Path.StartsWithSegments("/health")
    ? Serilog.Events.LogEventLevel.Verbose : Serilog.Events.LogEventLevel.Information);
});

// Configura el pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseExceptionHandler();

app.MapControllers(); // Busca los controladores y los usa

app.Run();