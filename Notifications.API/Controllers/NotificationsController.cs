using Microsoft.AspNetCore.Mvc;
using Notifications.API.DTOs;
using Notifications.API.Services;

namespace Notifications.API.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly NotificationService _notificationService;
    public NotificationsController(NotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    // Endpoint para enviar una notificación (POST /api/notifications/send)
    [HttpPost("send")]
    public ActionResult<NotificationResponse> SendNotification([FromBody] CreateNotificationRequest request)
    {
        var response = _notificationService.SendNotification(request);

        // Retorna la notificación creada con un código de estado 201 (Created)
        return CreatedAtAction(nameof(GetByUserId), new { userId = response.UsuarioId }, response);
    }

    // Endpoint para obtener las notificaciones de un usuario (GET /api/notifications/user/{userId})
    [HttpGet("{userId:guid}")]
    public IActionResult GetByUserId([FromRoute] Guid userId)
    {
        var response = _notificationService.GetNotificationsByUserId(userId);

        // Retorna la lista de notificaciones del usuario con un código de estado 200 (OK)
        return Ok(response);
    }
}
