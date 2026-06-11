using Microsoft.Data.Sqlite;
using Dapper;
using Users.API.Models;

namespace Users.API.Repositories
{
    public class UsersRepository
    {
        private readonly string _connectionString;

        public UsersRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("UsersDb");
        }

        public void Crear(User user)
        {
            string sql = @"
                INSERT INTO Users (Id, Nombre, Apellido, Email, PasswordHash, FechaRegistro, Activo, IntentosFallidos, MarcadoComoFraude)
                VALUES (@Id, @Nombre, @Apellido, @Email, @PasswordHash, @FechaRegistro, @Activo, @IntentosFallidos, @MarcadoComoFraude);
            ";

            SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();
            connection.Execute(sql, user);
            connection.Close();
        }

        public User ObtenerPorEmail(string email)
        {
            string sql = "SELECT * FROM Users WHERE Email = @Email";

            SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();
            User usuario = connection.QueryFirstOrDefault<User>(sql, new { Email = email });
            connection.Close();

            return usuario;
        }

        public User ObtenerPorId(Guid id)
        {
            string sql = "SELECT * FROM Users WHERE Id = @Id";

            SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();
            User usuario = connection.QueryFirstOrDefault<User>(sql, new { Id = id.ToString() });
            connection.Close();

            return usuario;
        }

        public void Actualizar(User user)
        {
            string sql = @"
                UPDATE Users
                SET Nombre = @Nombre,
                    Apellido = @Apellido,
                    Email = @Email,
                    PasswordHash = @PasswordHash,
                    Activo = @Activo,
                    IntentosFallidos = @IntentosFallidos,
                    MarcadoComoFraude = @MarcadoComoFraude
                WHERE Id = @Id;
            ";

            SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();
            connection.Execute(sql, user);
            connection.Close();
        }
    }
}