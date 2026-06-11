using Notifications.API.ExceptionHandlers;

var builder = WebApplication.CreateBuilder(args);

// Registro de servicios
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registro en orden de los manejadores de excepciones
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<BusinessRuleExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Formato estándar para respuestas de error (ProblemDetails)
builder.Services.AddProblemDetails();

// Registro de controladores
builder.Services.AddControllers();

// Configuración para que las respuestas de error de validación de modelo sean consistentes
builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(new
        {
            type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            title = "Bad Request",
            status = 400,
            detail = "Los datos de la notificación son inválidos.",
            instance = context.HttpContext.Request.Path.Value,
            errorCode = "NTF-002",
            errorMessage = "Los datos de la notificación son inválidos o el tipo no es reconocido."
        });
    };
});

// Registro de servicios de la aplicación
builder.Services.AddScoped<Notifications.API.Services.NotificationService>();

// Registro del repositorio de notificaciones para acceso a datos (SQLite)
builder.Services.AddScoped<Notifications.API.Repositories.NotificationRepository>();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configuración del middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

// Mapea las rutas definidas en los controladores
app.MapControllers();

app.Run();