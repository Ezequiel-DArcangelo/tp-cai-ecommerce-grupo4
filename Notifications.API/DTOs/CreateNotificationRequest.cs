using System.ComponentModel.DataAnnotations;

namespace Notifications.API.DTOs
{
    public class CreateNotificationRequest
    {
        [Required(ErrorMessage = "El campo UsuarioId es obligatorio.")]
        public Guid UsuarioId { get; set; }

        [Required(ErrorMessage = "El campo Mensaje es obligatorio.")]
        [StringLength(500, ErrorMessage = "El mensaje no puede exceder los 500 caracteres.")]
        public string Mensaje { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Tipo es obligatorio.")]
        [RegularExpression("^(Email|SMS|Push)$", ErrorMessage = "El tipo de notificación debe ser 'Email', 'SMS' o 'Push'.")]
        public string Tipo { get; set; } = string.Empty;
    }
}
