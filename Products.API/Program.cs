using Products.API.Services;
var builder = WebApplication.CreateBuilder(args);

// Agregamos servicios al contenedor
builder.Services.AddControllers(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ProductService>(); // Registro del servicio de productos como Scoped

var app = builder.Build();

// Configura el pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers(); // Busca los controladores y los usa

app.Run();