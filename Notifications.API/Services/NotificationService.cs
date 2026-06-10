using Notifications.API.DTOs;
using Notifications.API.Exceptions;
using Notifications.API.Models;

namespace Notifications.API.Services;

public class NotificationService
{
    // Historial temporal en memoria 
    private static readonly List<Notification> _notificationsDb = new();

    // Control de existencia de usuarios
    private static readonly List<Guid> _existingUsersMock = new()
    {
        Guid.Parse("a1b2c3d4-0000-0000-0000-111122223333")
    };

    // 1. Lógica para registrar (POST)
    public NotificationResponse SendNotification(CreateNotificationRequest request)
        {
        // Validación de existencia de usuario (Simulación del llamado a la API de Usuarios)
        if (!_existingUsersMock.Contains(request.UsuarioId))
        {
            throw new NotFoundException("NTF-001","El usuario destinatario no fue encontrado.");
        }

        // Si el usuario efectivamente existe, mapeamos el DTO al Modelo de dominio para guardarlo
        var newNotification = new Notification
        {
            UsuarioId = request.UsuarioId,
            Mensaje = request.Mensaje,
            Tipo = request.Tipo,
        };

        // Simulación de guardado en base de datos (en este caso, una lista en memoria)
        _notificationsDb.Add(newNotification);
     
        // Retornar respuesta
        return new NotificationResponse
        {
            Id = newNotification.Id,
            UsuarioId = newNotification.UsuarioId,
            Mensaje = newNotification.Mensaje,
            Tipo = newNotification.Tipo,
            FechaEnvio = newNotification.FechaEnvio
        };
    }
}
