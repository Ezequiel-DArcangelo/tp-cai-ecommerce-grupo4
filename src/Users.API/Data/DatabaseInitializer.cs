using Microsoft.Data.Sqlite;

namespace Users.API.Data
{
    public static class DatabaseInitializer
    {
        public static void Initialize(string connectionString)
        {
            // Abrir conexion a la base. Si el archivo .db no existe, SQLite lo crea.
            SqliteConnection connection = new SqliteConnection(connectionString);
            connection.Open();

            // Crear la tabla Users si no existe.
            // Los nombres de columnas coinciden con las propiedades del modelo User.
            string sql = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id TEXT PRIMARY KEY,
                    Nombre TEXT NOT NULL,
                    Apellido TEXT NOT NULL,
                    Email TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL,
                    FechaRegistro TEXT NOT NULL,
                    Activo INTEGER NOT NULL,
                    IntentosFallidos INTEGER NOT NULL,
                    MarcadoComoFraude INTEGER NOT NULL
                );
            ";

            SqliteCommand command = new SqliteCommand(sql, connection);
            command.ExecuteNonQuery();

            connection.Close();
        }
    }
}