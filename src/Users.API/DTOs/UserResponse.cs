namespace Users.API.DTOs
{
    /// <summary>
    /// Datos del usuario que se devuelven al cliente. NO incluye el PasswordHash por seguridad.
    /// </summary>
    public class UserResponse
    {
        /// <summary>
        /// Identificador único del usuario.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre del usuario.
        /// </summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Apellido del usuario.
        /// </summary>
        public string Apellido { get; set; } = string.Empty;

        /// <summary>
        /// Email del usuario.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de registro del usuario en el sistema (UTC).
        /// </summary>
        public DateTime FechaRegistro { get; set; }

        /// <summary>
        /// Indica si el usuario está activo. False cuando está bloqueado.
        /// </summary>
        public bool Activo { get; set; }
    }
}