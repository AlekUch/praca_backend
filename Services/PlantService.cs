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

        public async Task<List<PlantDTO>> GetPlants()
        {
            try
            {
                var plants = await _context.Plants
                     .Select(p => new PlantDTO
                     {
                         PlantId = p.PlantId,
                         Name = p.Name,
                         RotationPeriod = p.RotationPeriod,                        
                     })
                    .ToListAsync();
                return plants;
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
                    RotationPeriod = plantDto.RotationPeriod
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
    }
}
