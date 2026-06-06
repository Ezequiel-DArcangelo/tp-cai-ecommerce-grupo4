using Products.API.ExceptionHandlers;
using Products.API.Services;

var builder = WebApplication.CreateBuilder(args);

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
        problemDetails.Extensions["errorMessage"] = "Los datos del producto no son válidos.";

        return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(problemDetails);
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ProductService>(); // Registro del servicio de productos como Scoped
builder.Services.AddHttpClient(); // Para habilitar las llamadas HTTP desde el servicio de productos

// Manejadores de excepciones 
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<BusinessRuleExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

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