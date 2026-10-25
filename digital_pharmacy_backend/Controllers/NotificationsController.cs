using Application.Constants;
using Application.Models;
using digital_pharmacy_backend.Services.Firebase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace digital_pharmacy_backend.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize(Roles = "Administrator,Admin,Agent")]
public sealed class NotificationsController : ControllerBase
{
    private readonly IFirebaseNotificationService firebaseNotificationService;

    public NotificationsController(IFirebaseNotificationService firebaseNotificationService)
    {
        this.firebaseNotificationService = firebaseNotificationService;
    }

    [HttpPost("data-updated")]
    public async Task<IActionResult> SendDataUpdatedNotification(CancellationToken cancellationToken)
    {
        var result = await firebaseNotificationService.SendDataUpdatedAsync(cancellationToken);

        return Ok(ApiResponse<object>.Success(result, AppMessages.Success));
    }
}
