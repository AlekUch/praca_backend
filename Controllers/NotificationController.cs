using AGROCHEM.Models.EntitiesDto;
using AGROCHEM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AGROCHEM.Controllers
{
  
    [Route("agrochem/notifications")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService _notificationService;
        public NotificationController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetNotificationsStatus()
        {
            var userId = HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Ok(false);
            }

            try
            {
                bool result = await _notificationService.GetNotificationsStatus(Convert.ToInt32(userId));
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Wystąpił błąd podczas pobierania powiadomień." });
            }

        }
        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var userId = HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Ok(false);
            }

            try
            {
                var notifications = await _notificationService.GetNotifications(Convert.ToInt32(userId));
                return Ok(notifications);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Wystąpił błąd podczas pobierania powiadomień." });
            }

        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            try
            {
                bool isDeleted = await _notificationService.DeleteNotification(id);

                if (!isDeleted)
                {
                    return BadRequest(new { message = "Nie można usunąć powiadomienia." });
                }

                return Ok(new { message = "Usunięto pomyślnie" });

            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut]
        [Route("read/{id}")]
        public async Task<IActionResult> UpdateIsRead(int id, [FromQuery] bool isRead)
        {
            try
            {

                bool isUpdated = await _notificationService.UpdateRead(id, isRead);
                             
                if (isUpdated)
                {
                    return Ok(new { message = "Odczytano" });
                }
                else
                {
                    return BadRequest(new { message = "Błąd przetwarzania" });
                }

            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}

