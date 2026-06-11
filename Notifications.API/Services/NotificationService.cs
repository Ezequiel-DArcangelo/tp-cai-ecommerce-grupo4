using Notifications.API.DTOs;
using Notifications.API.Exceptions;
using Notifications.API.Models;
using Notifications.API.Repositories;

namespace Notifications.API.Services;

public class NotificationService
{
    private readonly NotificationRepository _notificationRepository;

    // Control de existencia de usuarios 
    private static readonly List<Guid> _existingUsersMock = new()
    {
        Guid.Parse("a1b2c3d4-0000-0000-0000-111122223333")
    };

    
    public NotificationService(NotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    // 1. Lógica para registrar/enviar (POST)
    public NotificationResponse SendNotification(CreateNotificationRequest request)
    {
        // Validamos la existencia de usuario (llamado a la API de Usuarios)
        if (!_existingUsersMock.Contains(request.UsuarioId))
        {
            throw new NotFoundException("NTF-001", "El usuario destinatario no fue encontrado.");
        }

        // Si el usuario existe, mapeamos el DTO al Modelo de dominio
        var newNotification = new Notification
        {
            Id = Guid.NewGuid(),
            UsuarioId = request.UsuarioId,
            Mensaje = request.Mensaje,
            Tipo = request.Tipo,
            Estado = "Pendiente",
            FechaEnvio = DateTime.UtcNow
        };

    
        _notificationRepository.Add(newNotification);

        // Retornamos respuesta formateada según el DTO de salida 
        return new NotificationResponse
        {
            Id = newNotification.Id,
            UsuarioId = newNotification.UsuarioId,
            Mensaje = newNotification.Mensaje,
            Tipo = newNotification.Tipo,
            Estado = "Pendiente",
            FechaEnvio = newNotification.FechaEnvio
        };
    }

    // 2. Lógica para listar por usuario (GET)
    public IEnumerable<NotificationResponse> GetNotificationsByUserId(Guid userId)
    {
       
        var userNotifications = _notificationRepository.GetByUserId(userId);

        // Si el usuario no tiene ninguna notificación registrada en la BD (NTF-003)
        if (!userNotifications.Any())
        {
            throw new NotFoundException("NTF-003", "No se encontraron notificaciones para el usuario especificado.");
        }

        // Mapeamos cada notificación encontrada a un DTO de respuesta
        return userNotifications.Select(n => new NotificationResponse
        {
            Id = n.Id,
            UsuarioId = n.UsuarioId,
            Mensaje = n.Mensaje,
            Tipo = n.Tipo,
            Estado = n.Estado,
            FechaEnvio = n.FechaEnvio
        });
    }
}