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

var app = builder.Build();

// Configuración del middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();


//...

app.Run();