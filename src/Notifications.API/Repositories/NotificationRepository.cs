using Microsoft.Data.Sqlite;
using System.Data;
using Dapper;
using Notifications.API.Models;

namespace Notifications.API.Repositories;

public class NotificationRepository
{
    private readonly string _connectionString;

    public NotificationRepository(IConfiguration configuration)
    {
        // Lee la ruta del archivo .db desde el appsettings.json
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;

        // Se inicializa la tabla automáticamente para que esté lista para usar
        InicializarBaseDeDatos();
    }

    private void InicializarBaseDeDatos()
    {
        using IDbConnection db = new SqliteConnection(_connectionString);

        // Crea la tabla de notificaciones si no existe, con los campos necesarios para almacenar la información de cada notificación
        string query = @"
            CREATE TABLE IF NOT EXISTS Notifications (
                Id TEXT PRIMARY KEY,
                UsuarioId TEXT NOT NULL,
                Mensaje TEXT NOT NULL,
                Tipo TEXT NOT NULL,
                Estado TEXT NOT NULL,
                FechaEnvio TEXT NOT NULL
            );";

        db.Execute(query);
    }

    // GUARDAR: se inserta la notificación en el archivo .db usando Dapper
    public void Add(Notification notification)
    {
        using IDbConnection db = new SqliteConnection(_connectionString);

        string query = @"
            INSERT INTO Notifications (Id, UsuarioId, Mensaje, Tipo, Estado, FechaEnvio) 
            VALUES (@Id, @UsuarioId, @Mensaje, @Tipo, @Estado, @FechaEnvio);";

        db.Execute(query, notification);
    }

    // BUSCAR: se trae la lista de notificaciones de un usuario desde el archivo .db
    public List<Notification> GetByUserId(Guid userId)
    {
        using IDbConnection db = new SqliteConnection(_connectionString);

        string query = "SELECT * FROM Notifications WHERE UsuarioId = @UserId;";

        return db.Query<Notification>(query, new { UsuarioId = userId.ToString() }).ToList();

    }
}
