using AGROCHEM.Data;
using AGROCHEM.Models.Entities;
using AGROCHEM.Models.EntitiesDto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AGROCHEM.Services
{
   
    public class PlantService
    {
        private readonly AgrochemContext _context;
        private readonly IConfiguration _configuration;

        public PlantService(AgrochemContext context,  IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }

        public async Task<List<PlantDTO>> GetPlants(bool isArchive)
        {
            try
            {
                if (isArchive == false)
                {
                    var plants = await _context.Plants
                        .Where(p => p.Archival == isArchive)
                         .Select(p => new PlantDTO
                         {
                             PlantId = p.PlantId,
                             Name = p.Name,
                             RotationPeriod = p.RotationPeriod,
                             Archival = p.Archival
                         })

                        .ToListAsync();
                    return plants;
                }
                else
                {
                    var plants = await _context.Plants
                        .Select(p => new PlantDTO
                        {
                            PlantId = p.PlantId,
                            Name = p.Name,
                            RotationPeriod = p.RotationPeriod,
                            Archival = p.Archival
                        })

                       .ToListAsync();
                    return plants;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                throw new ApplicationException("Błąd podczas pobierania działek", ex);
            }
        }

        public async Task<string> AddPlant(PlantDTO plantDto)
        {
            try
            {
                var plant = _context.Plants
                .FirstOrDefault(p => p.Name == plantDto.Name);
                if (plant != null)
                {
                    return "Roślina o tej nazwie juz istnieje.";
                }

               
                var newPlant = new Plant
                {
                    Name = plantDto.Name,
                    RotationPeriod = plantDto.RotationPeriod,
                    Archival = false
                   // Archival = false
                };

                _context.Plants.Add(newPlant);
                await _context.SaveChangesAsync();
                return "Utworzono roślinę.";
            }
            catch (Exception ex)
            {
                // Logowanie błędu
                Console.WriteLine(ex.Message);
                return ex.Message;
            }
        }
        public async Task<bool> UpdateArchivePlant(int id, bool archive)
        {
            try
            {
                var plant = await _context.Plants.FindAsync(id);
                if (plant == null)
                {
                    return false;
                }

                plant.Archival = archive;

                _context.Plants.Update(plant);
                await _context.SaveChangesAsync();

                return true; // Operacja zakończona sukcesem
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                throw new ApplicationException("Błąd podczas archiwizacj działek", ex);
            }
        }

        public async Task<bool> UpdatePlant(int id, PlantDTO plantDTO)
        {
            var plant = await _context.Plants.FindAsync(id);
            if (plant == null)
            {
                return false; 
            }

            plant.Name = plantDTO.Name;
            plant.RotationPeriod = plantDTO.RotationPeriod;
            
            _context.Plants.Update(plant);
            await _context.SaveChangesAsync();

            return true; // Operacja zakończona sukcesem
        }
    }
}
