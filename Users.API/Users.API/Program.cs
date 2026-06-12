using Users.API.Services;
using Users.API.ExceptionHandlers;
using Users.API.Data;
using Users.API.Repositories;
using Dapper;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

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
// Configuracion de Swagger con documentacion XML e informacion del API
builder.Services.AddSwaggerGen(options =>
{
    // Informacion general del API que aparece arriba en Swagger UI
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Users API",
        Version = "v1",
        Description = "API de gestión de usuarios del e-commerce. Permite registrar usuarios, autenticarlos y consultarlos por ID."
    });

    // Leer el archivo XML generado al compilar para mostrar los comentarios en Swagger
    string xmlFileName = System.AppDomain.CurrentDomain.FriendlyName + ".xml";
    string xmlPath = System.IO.Path.Combine(System.AppContext.BaseDirectory, xmlFileName);
    if (System.IO.File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});
builder.Services.AddSingleton<UsersRepository>();
builder.Services.AddSingleton<UsersService>();

// Registrar los handlers de excepciones (orden: del mas especifico al mas generico)
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<BusinessRuleExceptionHandler>();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Registrar los health checks. El check de SQLite se etiqueta como "ready"
// para que solo se evalue en /health/ready.
builder.Services.AddHealthChecks()
    .AddCheck<SqliteHealthCheck>("sqlite", tags: new[] { "ready" });


var app = builder.Build();

// Registrar el TypeHandler para convertir entre Guid (C#) y string (SQLite)
SqlMapper.AddTypeHandler(new GuidTypeHandler());

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

// Endpoints de health checks con respuesta en formato JSON
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = EscribirRespuestaHealthCheck
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = EscribirRespuestaHealthCheck
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => false,
    ResponseWriter = EscribirRespuestaHealthCheck
});

app.MapControllers();

app.Run();

// Metodo que arma la respuesta JSON de los health checks
static Task EscribirRespuestaHealthCheck(HttpContext context, HealthReport report)
{
    context.Response.ContentType = "application/json";

    var resultado = new
    {
        status = report.Status.ToString(),
        checks = report.Entries.Select(entry => new
        {
            name = entry.Key,
            status = entry.Value.Status.ToString(),
            description = entry.Value.Description
        }),
        totalDuration = report.TotalDuration.ToString()
    };

    return context.Response.WriteAsJsonAsync(resultado);
}