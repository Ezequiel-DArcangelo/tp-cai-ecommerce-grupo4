using System.ComponentModel.DataAnnotations;

namespace Users.API.DTOs
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La password es obligatoria.")]
        [MinLength(8, ErrorMessage = "La password debe tener al menos 8 caracteres.")]
        public string Password { get; set; } = string.Empty;
    }
}
