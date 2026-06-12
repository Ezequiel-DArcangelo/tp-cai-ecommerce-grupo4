using Cart.API.Data;
using Cart.API.ExceptionHandlers;
using Cart.API.HealthChecks;
using Cart.API.Middleware;
using Cart.API.Services;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Events;

// Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Diagnostics", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Servicio", "Cart.API")
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{Servicio}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/cart-.log",
        formatter: new Serilog.Formatting.Json.JsonFormatter(),
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

//Servicios 
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Cart API",
        Version = "v1",
        Description = "API para gestión del carrito de compras"
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});


// HTTP Client para Products API
builder.Services.AddHttpClient<ProductsApiClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7196");
});

builder.Services.AddSingleton<CartRepository>();
builder.Services.AddSingleton<CartDatabaseInitializer>();
builder.Services.AddScoped<CartService>();


builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<BusinessRuleExceptionHandler>();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<GenericExceptionHandler>();
builder.Services.AddProblemDetails();


builder.Services.AddHealthChecks()
    .AddCheck<SqliteHealthCheck>("sqlite-db", tags: ["database"])
    .AddCheck<ApiStatusCheck>("api-status", tags: ["api"]);

builder.Services.AddHealthChecksUI(setup =>
{
    setup.SetEvaluationTimeInSeconds(600);
    setup.AddHealthCheckEndpoint("Cart.API", "/health");
}).AddInMemoryStorage();

var app = builder.Build();

// Inicializar base de datos
using (var scope = app.Services.CreateScope())
    scope.ServiceProvider
        .GetRequiredService<CartDatabaseInitializer>()
        .Initialize();

// Pipeline
app.UseExceptionHandler();

app.UseSerilogRequestLogging(options =>
{
    options.GetLevel = (httpContext, _, ex) =>
        ex != null ? LogEventLevel.Error :
        httpContext.Request.Path.StartsWithSegments("/health")
            ? LogEventLevel.Verbose : LogEventLevel.Information;
});

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<AuditMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Health Check endpoints
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("database"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("api"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecksUI(setup => setup.UIPath = "/health-ui");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();