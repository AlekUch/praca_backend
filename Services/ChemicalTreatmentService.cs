using AGROCHEM.Data;
using AGROCHEM.Models.Entities;
using AGROCHEM.Models.EntitiesDto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace AGROCHEM.Services
{
    public class ChemicalTreatmentService
    {
        private readonly AgrochemContext _context;
        private readonly IConfiguration _configuration;

        public ChemicalTreatmentService(AgrochemContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }

        public async Task<List<ChemicalTreatmentGetDTO>> GetChemicalTreatments(int userId)
        {
            try
            {
                var chemTreat = await _context.ChemicalTreatments
                    .Include(p => p.Cultivation)
                    .Join(_context.Plots,
                         a => a.CultivationId,
                        b => b.PlotId,
                        (a, b) => new { a, b })
                    .Where(x => x.b.OwnerId == userId)
                    .Select(p => new ChemicalTreatmentGetDTO
                    {
                        ChemTreatId = p.a.ChemTreatId,
                        PlotNumber = p.a.Cultivation.Plot.PlotNumber,
                        PlantName = p.a.Cultivation.Plant.Name,
                        PlantId = p.a.Cultivation.Plant.PlantId,
                        CultivationId = p.a.CultivationId,
                        Date = p.a.Date,
                        Area = p.a.Area,
                        Dose = p.a.Dose,
                        Reason = p.a.Reason,
                        ChemAgentName=p.a.ChemAgent.Name,
                        ChemAgentId = p.a.ChemAgent.ChemAgentId,
                        ChemUseId = _context.ChemicalUses.Where(c => c.ChemAgentId == p.a.ChemAgentId && c.PlantId == p.a.Cultivation.Plant.PlantId).Select(c => c.ChemUseId).FirstOrDefault(),
                        MinDose = _context.ChemicalUses.Where(c => c.ChemAgentId == p.a.ChemAgentId && c.PlantId == p.a.Cultivation.Plant.PlantId).Select(c => c.MinDose).FirstOrDefault(),
                        MaxDose = _context.ChemicalUses.Where(c => c.ChemAgentId == p.a.ChemAgentId && c.PlantId == p.a.Cultivation.Plant.PlantId).Select(c => c.MaxDose).FirstOrDefault()
                    })
                    .OrderBy(p => p.Date)
                    .ToListAsync();
                return chemTreat;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                throw new ApplicationException("Błąd podczas pobierania działek", ex);
            }
        }

        public async Task<string> AddChemicalTreatment(ChemicalTreatmentDTO chemicalTreatmentDTO)
        {
            try
            {
                var chemTreat = _context.ChemicalTreatments
                .FirstOrDefault(p => p.Date == chemicalTreatmentDTO.Date && p.CultivationId == chemicalTreatmentDTO.CultivationId && p.Dose==chemicalTreatmentDTO.Dose);
                if (chemTreat != null)
                {
                    return "Taki zabieg już istnieje.";
                }


                var newChemTreat = new ChemicalTreatment
                {
                    ChemAgentId = chemicalTreatmentDTO.ChemAgentId,
                    CultivationId = chemicalTreatmentDTO.CultivationId,
                    Date = chemicalTreatmentDTO.Date,
                    Dose = chemicalTreatmentDTO.Dose,
                    Reason = chemicalTreatmentDTO.Reason,
                    Area = chemicalTreatmentDTO.Area
                   
                };

                _context.ChemicalTreatments.Add(newChemTreat);
                await _context.SaveChangesAsync();
                return "Utworzono nowy zabieg chemiczny.";
            }
            catch (Exception ex)
            {
                // Logowanie błędu
                Console.WriteLine(ex.Message);
                return ex.Message;
            }
        }

        public async Task<bool> UpdateChemTreat(int id, ChemicalTreatmentDTO chemTreatDTO)
        {
            var chemTreat = await _context.ChemicalTreatments.FindAsync(id);
            if (chemTreat == null)
            {
                return false;
            }

            chemTreat.CultivationId = chemTreatDTO.CultivationId;
            chemTreat.Date = chemTreatDTO.Date;
            chemTreat.Area = chemTreatDTO.Area;
            chemTreat.ChemAgentId = chemTreatDTO.ChemAgentId;
            chemTreat.Dose = chemTreatDTO.Dose;
            chemTreat.Reason = chemTreatDTO.Reason;

            _context.ChemicalTreatments.Update(chemTreat);
            await _context.SaveChangesAsync();

            return true; // Operacja zakończona sukcesem
        }

    }
}
