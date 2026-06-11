using Notifications.API.DTOs;
using Notifications.API.Exceptions;
using Notifications.API.Models;
using Notifications.API.Repositories;

namespace Notifications.API.Services;

public class NotificationService
{
    private readonly NotificationRepository _notificationRepository;
    private readonly IHttpClientFactory _httpClientFactory;

    public NotificationService(NotificationRepository notificationRepository, IHttpClientFactory httpClientFactory)
    {
        _notificationRepository = notificationRepository;
        _httpClientFactory = httpClientFactory;
    }

    private async Task<bool> ExisteUsuario(Guid usuarioId)
    {
        HttpClient client = _httpClientFactory.CreateClient();

        string url = "https://localhost:7206/api/users/" + usuarioId.ToString();

        HttpResponseMessage respuesta = await client.GetAsync(url);

        // Si la API de Usuarios devuelve 404, el usuario no existe en el sistema 
        if (respuesta.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false; 
        }

        return true;
    }

    

    // 1. Lógica para registrar/enviar (POST)
    public async Task<NotificationResponse> SendNotificationAsync(CreateNotificationRequest request)
    {
        // Llamada HTTP a la API de Usuarios
        bool usuarioExiste = await ExisteUsuario(request.UsuarioId);
        if (!usuarioExiste)
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