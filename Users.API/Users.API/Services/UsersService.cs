using Users.API.DTOs;
using Users.API.Models;
using Users.API.Exceptions;


namespace Users.API.Services
{
    public class UsersService
    {
        // Lista en memoria que simula la base de datos
        private static List<User> _users = new List<User>();

        public UserResponse Register(RegisterRequest request)
        {
            // Verificar que el email no este registrado (USR-001)
            foreach (User user in _users)
            {
                if (user.Email == request.Email)
                {
                    throw new BusinessRuleException(
                        "USR-001",
                        "El email '" + request.Email + "' ya está registrado.");
                }
            }

            // Crear el nuevo usuario
            User newUser = new User();
            newUser.Id = Guid.NewGuid();
            newUser.Nombre = request.Nombre;
            newUser.Apellido = request.Apellido;
            newUser.Email = request.Email;
            newUser.PasswordHash = request.Password; // TODO: hashear
            newUser.FechaRegistro = DateTime.UtcNow;
            newUser.Activo = true;
            newUser.IntentosFallidos = 0;

            // Agregarlo a la lista
            _users.Add(newUser);

            // Devolver la respuesta sin el PasswordHash
            return MapToResponse(newUser);
        }

        public UserResponse Login(LoginRequest request)
        {
            // Buscar el usuario por email
            User userEncontrado = null;
            foreach (User user in _users)
            {
                if (user.Email == request.Email)
                {
                    userEncontrado = user;
                    break;
                }
            }

            // Si no existe el usuario
            if (userEncontrado == null)
            {
                return null;
            }

            // Verificar la contraseña
            if (userEncontrado.PasswordHash != request.Password)
            {
                return null;
            }

            // Devolver respuesta sin PasswordHash
            return MapToResponse(userEncontrado);
        }

        // Método auxiliar: convierte User en UserResponse
        private UserResponse MapToResponse(User user)
        {
            UserResponse response = new UserResponse();
            response.Id = user.Id;
            response.Nombre = user.Nombre;
            response.Apellido = user.Apellido;
            response.Email = user.Email;
            response.FechaRegistro = user.FechaRegistro;
            response.Activo = user.Activo;
            return response;
        }
    }
}