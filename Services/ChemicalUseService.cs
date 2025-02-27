using AGROCHEM.Data;
using AGROCHEM.Models.Entities;
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
                         ChemAgentId =p.ChemAgentId,
                         PlantId = p.PlantId,
                         MinDose = p.MinDose,
                         MaxDose = p.MaxDose,
                         MinWater = p.MinWater,
                         MaxWater = p.MaxWater,
                         MinDays = p.MinDays,
                         MaxDays = p.MaxDays,
                         PlantName = p.Plant.Name,
                         NumberOfTreatments = p.NumberOfTreatments,
                         Archival =p.Archival
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

        public async Task<List<ChemicalUseDTO>> GetChemAgentUsePlant(int id)
        {
            try
            {

                var chemAgentUse = await _context.ChemicalUses
                    .Where(p => p.PlantId == id)
                    .Include(p => p.Plant)
                    .Include(p=>p.ChemAgent)
                     .Select(p => new ChemicalUseDTO
                     {
                         ChemUseId = p.ChemUseId,
                         ChemAgentId = p.ChemAgentId,
                         ChemAgentName = p.ChemAgent.Name,
                         PlantId = p.PlantId,
                         MinDose = p.MinDose,
                         MaxDose = p.MaxDose,
                         MinWater = p.MinWater,
                         MaxWater = p.MaxWater,
                         MinDays = p.MinDays,
                         MaxDays = p.MaxDays,
                         PlantName = p.Plant.Name,
                         NumberOfTreatments = p.NumberOfTreatments,
                         Archival = p.Archival
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

        public async Task<string> AddChemicalUse(ChemicalUseDTO chemicalUseDTO)
        {
            try
            {
                var chemUse = _context.ChemicalUses
                .FirstOrDefault(p => p.ChemAgentId == chemicalUseDTO.ChemAgentId && p.PlantId == chemicalUseDTO.PlantId);
                if (chemUse != null)
                {
                    return "Dawkowanie dla tej rośliny już istnieje.";
                }


                var newChemUse = new ChemicalUse
                {
                    ChemAgentId= chemicalUseDTO.ChemAgentId,
                    MinDose = chemicalUseDTO.MinDose,
                    MaxDose = chemicalUseDTO.MaxDose,
                    MinWater = chemicalUseDTO.MinWater,
                    MaxWater = chemicalUseDTO.MaxWater,
                    MinDays = chemicalUseDTO.MinDays,
                    MaxDays= chemicalUseDTO.MaxDays,
                    PlantId = chemicalUseDTO .PlantId,
                    NumberOfTreatments = chemicalUseDTO.NumberOfTreatments,
                    Archival = false
                };

                _context.ChemicalUses.Add(newChemUse);
                await _context.SaveChangesAsync();
                return "Utworzono nową informację.";
            }
            catch (Exception ex)
            {
                // Logowanie błędu
                Console.WriteLine(ex.Message);
                return ex.Message;
            }
        }

        public async Task<bool> UpdateChemicalUse(int id, ChemicalUseDTO chemicalUseDTO)
        {
            var chemUse = await _context.ChemicalUses.FindAsync(id);
            if (chemUse == null)
            {
                return false;
            }

            chemUse.PlantId = chemicalUseDTO.PlantId;
            chemUse.MinDose = chemicalUseDTO.MinDose;
            chemUse.MaxDose = chemicalUseDTO.MaxDose;
            chemUse.MinWater = chemicalUseDTO.MinWater;
            chemUse.MaxWater = chemicalUseDTO.MaxWater;
            chemUse.MinDays = chemicalUseDTO.MinDays;
            chemUse.MaxDays = chemicalUseDTO.MaxDays;
            chemUse.NumberOfTreatments = chemicalUseDTO.NumberOfTreatments;

            _context.ChemicalUses.Update(chemUse);
            await _context.SaveChangesAsync();

            return true; 
        }

        public async Task<bool> UpdateArchiveChemUse(int id, bool archive)
        {
            try
            {
                var chemUse = await _context.ChemicalUses.FindAsync(id);
                if (chemUse == null)
                {
                    return false;
                }
                chemUse.Archival = archive;

                _context.ChemicalUses.Update(chemUse);
                await _context.SaveChangesAsync();

                return true; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                throw new ApplicationException("Błąd podczas archiwizacj zastosowania", ex);
            }
        }
    }
}
