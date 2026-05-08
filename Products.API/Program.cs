var builder = WebApplication.CreateBuilder(args);

// 1. Agregar servicios al contenedor
builder.Services.AddControllers(); // Esto es clave para que reconozca tu ProductsController
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 2. Configurar el pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers(); // Esto le dice a la app: "Buscá mis controladores y usalos"

app.Run();