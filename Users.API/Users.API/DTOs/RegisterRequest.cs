using System.ComponentModel.DataAnnotations;

namespace Users.API.DTOs
{
    /// <summary>
    /// Datos necesarios para registrar un nuevo usuario.
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// Nombre del usuario. Campo obligatorio.
        /// </summary>
        /// <example>María</example>
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Apellido del usuario. Campo obligatorio.
        /// </summary>
        /// <example>González</example>
        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string Apellido { get; set; } = string.Empty;

        /// <summary>
        /// Email del usuario. Debe ser único y tener formato válido.
        /// </summary>
        /// <example>maria@email.com</example>
        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Password del usuario. Debe tener al menos 8 caracteres.
        /// </summary>
        /// <example>MiPassword123</example>
        [Required(ErrorMessage = "La password es obligatoria.")]
        [MinLength(8, ErrorMessage = "La password debe tener al menos 8 caracteres.")]
        public string Password { get; set; } = string.Empty;
    }
}