using Users.API.Services;
using Users.API.ExceptionHandlers;
using Users.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Desactivar la validacion automatica de Data Annotations.
// Las validaciones las maneja UsersService y devuelven ValidationException (USR-002).
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<UsersService>();

// Registrar los handlers de excepciones (orden: del mas especifico al mas generico)
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<BusinessRuleExceptionHandler>();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();


var app = builder.Build();

// Inicializar la base de datos (crea el archivo .db y la tabla Users si no existen)
string connectionString = builder.Configuration.GetConnectionString("UsersDb");
DatabaseInitializer.Initialize(connectionString);

// Activar el manejo global de excepciones (usa los handlers registrados arriba)
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
