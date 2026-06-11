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

// Registro de servicios de la aplicación
builder.Services.AddScoped<Notifications.API.Services.NotificationService>();

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