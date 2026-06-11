using Users.API.DTOs;
using Users.API.Models;
using Users.API.Exceptions;
using Users.API.Repositories;
using BCrypt.Net;

namespace Users.API.Services
{
    public class UsersService
    {
        private readonly UsersRepository _repository;

        public UsersService(UsersRepository repository)
        {
            _repository = repository;
        }

        public UserResponse Register(RegisterRequest request)
        {
            // Validar campos (USR-002)
            ValidarRegisterRequest(request);

            // Verificar que el email no este registrado (USR-001)
            User usuarioExistente = _repository.ObtenerPorEmail(request.Email);
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
            newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            newUser.FechaRegistro = DateTime.UtcNow;
            newUser.Activo = true;
            newUser.IntentosFallidos = 0;
            newUser.MarcadoComoFraude = false;

            // Persistir el usuario en la base
            _repository.Crear(newUser);

            return MapToResponse(newUser);
        }

        public UserResponse Login(LoginRequest request)
        {
            // Buscar el usuario por email
            User usuarioEncontrado = _repository.ObtenerPorEmail(request.Email);

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

                // Persistir los cambios en la base
                _repository.Actualizar(usuarioEncontrado);

                throw new BusinessRuleException("USR-003", "Credenciales incorrectas.");
            }

            // Login exitoso: resetear intentos fallidos
            usuarioEncontrado.IntentosFallidos = 0;

            // Persistir el reseteo en la base
            _repository.Actualizar(usuarioEncontrado);

            return MapToResponse(usuarioEncontrado);
        }

        public UserResponse ObtenerPorId(Guid id)
        {
            // Buscar el usuario por Id en la base
            User usuarioEncontrado = _repository.ObtenerPorId(id);

            // Si no existe, tirar NotFoundException con USR-007
            if (usuarioEncontrado == null)
            {
                throw new NotFoundException("USR-007", "Usuario no encontrado.");
            }

            return MapToResponse(usuarioEncontrado);
        }

        private void ValidarRegisterRequest(RegisterRequest request)
        {
            List<string> errores = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Nombre))
            {
                errores.Add("El nombre es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(request.Apellido))
            {
                errores.Add("El apellido es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                errores.Add("El email es obligatorio.");
            }
            else if (!request.Email.Contains("@") || !request.Email.Contains("."))
            {
                errores.Add("El email no tiene un formato válido.");
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                errores.Add("La password es obligatoria.");
            }
            else if (request.Password.Length < 8)
            {
                errores.Add("La password debe tener al menos 8 caracteres.");
            }

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