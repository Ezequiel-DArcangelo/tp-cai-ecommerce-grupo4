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
            // Validar campos (USR-002)
            ValidarRegisterRequest(request);

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

        // Valida los campos del request de registro.
        // Si hay errores, los acumula y lanza una sola ValidationException (USR-002).
        private void ValidarRegisterRequest(RegisterRequest request)
        {
            List<string> errores = new List<string>();

            // Validar Nombre
            if (string.IsNullOrWhiteSpace(request.Nombre))
            {
                errores.Add("El nombre es obligatorio.");
            }

            // Validar Apellido
            if (string.IsNullOrWhiteSpace(request.Apellido))
            {
                errores.Add("El apellido es obligatorio.");
            }

            // Validar Email
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                errores.Add("El email es obligatorio.");
            }
            else if (!request.Email.Contains("@") || !request.Email.Contains("."))
            {
                errores.Add("El email no tiene un formato válido.");
            }

            // Validar Password
            if (string.IsNullOrWhiteSpace(request.Password))
            {
                errores.Add("La password es obligatoria.");
            }
            else if (request.Password.Length < 8)
            {
                errores.Add("La password debe tener al menos 8 caracteres.");
            }

            // Si hay errores, lanzar una unica excepcion con todos juntos
            if (errores.Count > 0)
            {
                string mensaje = "";
                for (int i = 0; i < errores.Count; i++)
                {
                    mensaje = mensaje + errores[i];
                    if (i < errores.Count - 1)
                    {
                        mensaje = mensaje + " ";
                    }
                }

                throw new ValidationException("USR-002", mensaje);
            }
        }
    }
}