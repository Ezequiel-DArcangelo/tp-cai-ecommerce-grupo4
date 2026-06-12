using Microsoft.AspNetCore.Mvc;
using Users.API.DTOs;
using Users.API.Services;

namespace Users.API.Controllers
{
    /// <summary>
    /// Endpoints para gestion de usuarios: registro, autenticacion y consulta por ID.
    /// </summary>
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UsersService _usersService;

        public UsersController(UsersService usersService)
        {
            _usersService = usersService;
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema.
        /// </summary>
        /// <param name="request">Datos del usuario: nombre, apellido, email y password.</param>
        /// <returns>El usuario creado (sin la password).</returns>
        /// <response code="201">Usuario registrado correctamente.</response>
        /// <response code="400">Los datos enviados son inválidos (USR-002).</response>
        /// <response code="409">El email ya está registrado (USR-001).</response>
        /// <response code="500">Error interno al procesar el usuario (USR-006).</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(UserResponse), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            UserResponse response = _usersService.Register(request);
            return Created("", response);
        }

        /// <summary>
        /// Autentica un usuario con email y password.
        /// </summary>
        /// <param name="request">Credenciales del usuario: email y password.</param>
        /// <returns>Los datos del usuario autenticado (sin la password).</returns>
        /// <response code="200">Login exitoso.</response>
        /// <response code="400">Los datos enviados son inválidos (USR-002).</response>
        /// <response code="401">Credenciales incorrectas (USR-003).</response>
        /// <response code="403">Usuario bloqueado por intentos fallidos (USR-004) o por fraude (USR-005).</response>
        /// <response code="500">Error interno al procesar el usuario (USR-006).</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(UserResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            UserResponse response = _usersService.Login(request);
            return Ok(response);
        }

        /// <summary>
        /// Obtiene un usuario por su ID.
        /// </summary>
        /// <param name="id">Identificador único del usuario (Guid).</param>
        /// <returns>Los datos del usuario (sin la password).</returns>
        /// <response code="200">Usuario encontrado.</response>
        /// <response code="404">Usuario no encontrado (USR-007).</response>
        /// <response code="500">Error interno al procesar el usuario (USR-006).</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserResponse), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult ObtenerPorId(Guid id)
        {
            UserResponse response = _usersService.ObtenerPorId(id);
            return Ok(response);
        }
    }
}