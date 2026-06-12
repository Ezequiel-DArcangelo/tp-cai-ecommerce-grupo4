using System.ComponentModel.DataAnnotations;

namespace Users.API.DTOs
{
    /// <summary>
    /// Credenciales para autenticar un usuario.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Email del usuario registrado.
        /// </summary>
        /// <example>maria@email.com</example>
        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Password del usuario.
        /// </summary>
        /// <example>MiPassword123</example>
        [Required(ErrorMessage = "La password es obligatoria.")]
        public string Password { get; set; } = string.Empty;
    }
}