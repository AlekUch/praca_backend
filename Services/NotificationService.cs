using AGROCHEM.Data;
using AGROCHEM.Models.EntitiesDto;
using Microsoft.EntityFrameworkCore;

namespace AGROCHEM.Services
{
    public class NotificationService
    {
        private readonly AgrochemContext _context;
        private readonly IConfiguration _configuration;

        public NotificationService(AgrochemContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }

        public async Task<bool> GetNotificationsStatus(int userId)
        {
            try
            {
                var notifications = await _context.Notifications                     
                .Where(n => n.UserId == userId && n.IsRead == false && n.StartDate<= DateTime.Now.AddDays(-1))
                .ToListAsync();

                if (notifications.Any())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                throw new ApplicationException("Błąd podczas pobierania działek", ex);
            }
        }

        public async Task<List<NotificationDTO>> GetNotifications(int userId)
        {
            try
            {
                var notifications = await _context.Notifications
                .Where(n => n.UserId == userId &&  n.StartDate <= DateTime.Now.AddDays(-1))
                .Select(n=> new NotificationDTO
                {
                    NotificationId = n.NotificationId,
                    PlotNumber = n.Cultivation.Plot.PlotNumber,
                    PlantName = n.Cultivation.Plant.Name,
                    ChemAgentName = n.ChemAgent.Name,
                    StartDate = n.StartDate,
                    EndDate = n.EndDate,
                    IsRead = n.IsRead
                })
                .OrderByDescending(n=>n.StartDate)
                .ToListAsync();
                return notifications;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                throw new ApplicationException("Błąd podczas pobierania działek", ex);
            }
        }

        public async Task<bool> DeleteNotification(int id)
        {
            try
            {
                var notification = await _context.Notifications.FindAsync(id);
                if (notification == null)
                {
                    return false;
                }

                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();

                return true; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                throw new ApplicationException("Błąd podczas archiwizacj działek", ex);
            }
        }

        public async Task<bool> UpdateRead(int id, bool isRead)
        {
            try
            {
                var notification = await _context.Notifications.FindAsync(id);
                if (notification == null)
                {
                    return false;
                }

                notification.IsRead = isRead;

                _context.Notifications.Update(notification);
                await _context.SaveChangesAsync();

                return true; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                throw new ApplicationException("Błąd podczas archiwizacj działek", ex);
            }
        }
    }
}
