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
                    .Where(c => c.Cultivation.Plot.OwnerId == 1)
                    .Select(c => new ChemicalTreatmentGetDTO
                    {
                        ChemTreatId = c.ChemTreatId,
                        PlotNumber = c.Cultivation.Plot.PlotNumber,
                        PlantName = c.Cultivation.Plant.Name,
                        PlantId = c.Cultivation.Plant.PlantId,
                        CultivationId = c.CultivationId,
                        Date = c.Date,
                        Area = c.Area,
                        Dose = c.Dose,
                        Reason = c.Reason,
                        MaxArea = c.Cultivation.Area,
                        ChemAgentName = c.ChemAgent != null ? c.ChemAgent.Name : null,
                        ChemAgentId = c.ChemAgentId,
                        ChemUseId = _context.ChemicalUses
                        .Where(cu => (cu.ChemAgentId == c.ChemAgentId )
                            && (cu.PlantId == c.Cultivation.Plant.PlantId ))
                        .Select(cu => cu.ChemUseId)
                        .FirstOrDefault(),
                        MinDose = _context.ChemicalUses
                        .Where(cu => (cu.ChemAgentId == c.ChemAgentId )
                            && (cu.PlantId == c.Cultivation.Plant.PlantId ))
                        .Select(cu => cu.MinDose)
                        .FirstOrDefault(),
                        MaxDose = _context.ChemicalUses
                        .Where(cu => (cu.ChemAgentId == c.ChemAgentId )
                            && (cu.PlantId == c.Cultivation.Plant.PlantId ))
                        .Select(cu => cu.MaxDose)
                        .FirstOrDefault()
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

        public async Task<string> AddChemicalTreatment(ChemicalTreatmentDTO chemicalTreatmentDTO, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
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

                    int count = _context.ChemicalTreatments
                    .Where(p => p.ChemAgentId == chemicalTreatmentDTO.ChemAgentId && p.CultivationId == chemicalTreatmentDTO.CultivationId && p.Date.Value.Year == DateTime.Now.Year)
                    .Count();

                    int plantId = _context.Cultivations
                    .Where(p => p.CultivationId == chemicalTreatmentDTO.CultivationId)
                    .Select(p => (int)p.PlantId)
                    .FirstOrDefault();

                    var chemUse = _context.ChemicalUses
                        .Where(c => c.PlantId == plantId && c.ChemAgentId == chemicalTreatmentDTO.ChemAgentId)
                        .FirstOrDefault();

                    if (chemUse != null && count < chemUse.NumberOfTreatments)
                    {
                        DateTime? endDateToAdd = chemUse.MaxDays == null ?
                            null :
                            newChemTreat.Date.Value.AddDays((double)chemUse.MaxDays);
                        var notification = new Notification
                        {
                            UserId = userId,
                            ChemAgentId = chemicalTreatmentDTO.ChemAgentId,
                            CultivationId = chemicalTreatmentDTO.CultivationId,
                            StartDate = newChemTreat.Date.Value.AddDays((chemUse.MinDays ?? 0)),
                            EndDate = endDateToAdd
                        };
                        _context.Notifications.Add(notification);
                        await _context.SaveChangesAsync();
                    }
                await transaction.CommitAsync();
                return "Utworzono nowy zabieg chemiczny.";
                
            }
            
                catch (Exception ex)
                {
                    // Logowanie błędu
                    Console.WriteLine(ex.Message);
                    await transaction.RollbackAsync();
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

        public async Task<bool> DeleteChemTreat(int id)
        {
            try
            {
                var chemTreat = await _context.ChemicalTreatments.FindAsync(id);
                if (chemTreat == null)
                {
                    return false;
                }
               
                try
                {                    
                    _context.ChemicalTreatments.Remove(chemTreat);
                    await _context.SaveChangesAsync();                  
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);                 
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                throw new ApplicationException("Błąd usuwania zabiegu", ex);
            }
        }

    }
}
