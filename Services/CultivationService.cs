using AGROCHEM.Data;
using AGROCHEM.Models.Entities;
using AGROCHEM.Models.EntitiesDto;
using Microsoft.EntityFrameworkCore;

namespace AGROCHEM.Services
{
    public class CultivationService
    {
        private readonly AgrochemContext _context;
        private readonly IConfiguration _configuration;

        public CultivationService(AgrochemContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }

        public async Task<List<CultivationDTO>> GetCultivations(int userId)
        {
            try
            {
                var cultivations = await _context.Cultivations
                    .Include(c => c.Plot) // Zależność od Plot
                    .Include(c => c.Plant) // Zależność od Plant
                    .Where(c => c.Plot.OwnerId == userId && c.HarvestDate == null)
                    .Select(p => new CultivationDTO
                    {
                        CultivationId = p.CultivationId,
                        PlantName = p.Plant.Name,
                        PlotNumber = p.Plot.PlotNumber,
                        SowingDate = p.SowingDate,
                        HarvestDate = p.HarvestDate,
                        Area = p.Area,
                        Archival = p.Archival,
                        PlantId = p.PlantId,
                        PlotId = p.PlotId
                    })
                    .ToListAsync();
                return cultivations;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                throw new ApplicationException("Błąd podczas pobierania działek", ex);
            }
        }

        public async Task<List<Plant>> GetPlants()
        {
            try
            {
                var plants = await _context.Plants
                    .Where(p=>p.Archival==false)
                    .ToListAsync();


                return plants;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                throw new ApplicationException("Błąd podczas pobierania działek", ex);
            }
        }

        public async Task<string> AddCultivation(Cultivation cultivation)
        {
            try
            {
                var newCultivation = new Cultivation
                {
                    PlotId = cultivation.PlotId,
                    PlantId = cultivation.PlantId,
                    SowingDate = cultivation.SowingDate,
                    Area = cultivation.Area
                };

                _context.Cultivations.Add(newCultivation);
                await _context.SaveChangesAsync();

                return "Utworzono uprawę.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                throw new ApplicationException("Błąd podczas tworzenia uprawy", ex);
            }
        }
        public async Task<bool> UpdateCultivation(int id, CultivationDTO cultivationDTO)
        {
            var cultivation = await _context.Cultivations.FindAsync(id);
            if (cultivation == null)
            {
                return false; // Użytkownik nie istnieje
            }

            cultivation.PlotId = cultivationDTO.PlotId;
            cultivation.PlantId = cultivationDTO.PlantId;
            cultivation.Area = cultivationDTO.Area;
            cultivation.SowingDate = cultivationDTO.SowingDate;

            _context.Cultivations.Update(cultivation);
            await _context.SaveChangesAsync();

            return true; // Operacja zakończona sukcesem
        }

        public async Task<bool> UpdateArchiveCultivation(int id, bool archive)
        {
            try
            {
                var cultivation = await _context.Cultivations.FindAsync(id);
                if (cultivation == null)
                {
                    return false;
                }

                cultivation.Archival = archive;

                _context.Cultivations.Update(cultivation);
                await _context.SaveChangesAsync();

                return true; // Operacja zakończona sukcesem
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                throw new ApplicationException("Błąd podczas archiwizacj działek", ex);
            }
        }

        public async Task<bool> DeleteCultivation(int id)
        {
            try
            {
                var cultivation = await _context.Cultivations.FindAsync(id);
                if (cultivation == null)
                {
                    return false;
                }

                _context.Cultivations.Remove(cultivation);
                await _context.SaveChangesAsync();

                return true; // Operacja zakończona sukcesem
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                throw new ApplicationException("Błąd podczas archiwizacj działek", ex);
            }
        }
    }
}
