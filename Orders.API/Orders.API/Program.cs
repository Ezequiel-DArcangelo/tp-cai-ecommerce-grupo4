using Orders.API.Services;
using Orders.API.ExceptionHandlers;

var builder = WebApplication.CreateBuilder(args);

// Desactivar la validación automática de Data Annotations.
// Las validaciones las maneja OrdersService y devuelven ValidationException (ORD-002).
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

// Configuración de Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrar el servicio de órdenes
builder.Services.AddSingleton<OrdersService>();

// Registrar los handlers de excepciones (orden: del más específico al más genérico)
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<BusinessRuleExceptionHandler>();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<GenericExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Activar el manejo global de excepciones (usa los handlers registrados arriba)
app.UseExceptionHandler();

// Configuración del pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();