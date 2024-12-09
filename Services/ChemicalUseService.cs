using AGROCHEM.Data;
using AGROCHEM.Models.EntitiesDto;
using Microsoft.EntityFrameworkCore;

namespace AGROCHEM.Services
{
    public class ChemicalUseService
    {
        private readonly AgrochemContext _context;
        private readonly IConfiguration _configuration;

        public ChemicalUseService(AgrochemContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }

        public async Task<List<ChemicalUseDTO>> GetChemAgentUse(int id)
        {
            try
            {

                var chemAgentUse = await _context.ChemicalUses
                    .Where(p=>p.ChemAgentId == id)
                    .Include(p=>p.Plant)
                     .Select(p => new ChemicalUseDTO
                     {
                         ChemUseId = p.ChemUseId,
                         PlantId = p.PlantId,
                         MinDose = p.MinDose,
                         MaxDose = p.MaxDose,
                         MinWater = p.MinWater,
                         MaxWater = p.MaxWater,
                         MinDays = p.MinDays,
                         MaxDays = p.MaxDays,
                         PlantName = p.Plant.Name
                     })

                    .ToListAsync();
                return chemAgentUse;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                throw new ApplicationException("Błąd podczas pobierania szczegółowych informacji", ex);
            }
        }
    }
}
