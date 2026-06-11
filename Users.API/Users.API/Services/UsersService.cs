using Users.API.DTOs;
using Users.API.Models;
using Users.API.Exceptions;
using BCrypt.Net;


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
            User usuarioExistente = BuscarUsuarioPorEmail(request.Email);
            if (usuarioExistente != null)
            {
                throw new BusinessRuleException(
                    "USR-001",
                    "El email '" + request.Email + "' ya está registrado.");
            }

            // Crear el nuevo usuario
            User newUser = new User();
            newUser.Id = Guid.NewGuid();
            newUser.Nombre = request.Nombre;
            newUser.Apellido = request.Apellido;
            newUser.Email = request.Email;
            // Hashear la password antes de guardarla. BCrypt genera el salt automaticamente.
            newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
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
            User usuarioEncontrado = BuscarUsuarioPorEmail(request.Email);

            // Si no existe, devolvemos USR-003 (no USR-001 por seguridad: no revelamos si el email esta registrado)
            if (usuarioEncontrado == null)
            {
                throw new BusinessRuleException("USR-003", "Credenciales incorrectas.");
            }

            // Si esta marcado como fraude, USR-005 (chequea primero porque es mas grave que USR-004)
            if (usuarioEncontrado.MarcadoComoFraude == true)
            {
                throw new BusinessRuleException(
                    "USR-005",
                    "Su cuenta fue suspendida por razones de seguridad. Contacte a soporte.");
            }

            // Si esta bloqueado por intentos fallidos, USR-004
            if (usuarioEncontrado.Activo == false && usuarioEncontrado.IntentosFallidos >= 3)
            {
                throw new BusinessRuleException(
                    "USR-004",
                    "Su cuenta fue bloqueada por superar el máximo de intentos fallidos. Contacte a soporte.");
            }

            // Verificar la password usando BCrypt (compara contra el hash guardado)
            if (BCrypt.Net.BCrypt.Verify(request.Password, usuarioEncontrado.PasswordHash) == false)
            {
                // Password incorrecta: sumar intento fallido
                usuarioEncontrado.IntentosFallidos = usuarioEncontrado.IntentosFallidos + 1;

                // Si llego a 3 intentos fallidos, marcar como bloqueado
                if (usuarioEncontrado.IntentosFallidos >= 3)
                {
                    usuarioEncontrado.Activo = false;
                }

                // Devolver USR-003 igual (al siguiente intento ya recibira USR-004 si quedo bloqueado)
                throw new BusinessRuleException("USR-003", "Credenciales incorrectas.");
            }

            // Login exitoso: resetear intentos fallidos
            usuarioEncontrado.IntentosFallidos = 0;

            return MapToResponse(usuarioEncontrado);
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

        // Busca un usuario por email en la lista. Devuelve null si no existe.
        private User BuscarUsuarioPorEmail(string email)
        {
            foreach (User user in _users)
            {
                if (user.Email == email)
                {
                    return user;
                }
            }
            return null;
        }


    }
}