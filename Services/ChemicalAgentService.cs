using AGROCHEM.Data;
using AGROCHEM.Models.Entities;
using AGROCHEM.Models.EntitiesDto;
using Microsoft.EntityFrameworkCore;

namespace AGROCHEM.Services
{

    public class ChemicalAgentService
    {
        private readonly AgrochemContext _context;
        private readonly IConfiguration _configuration;

        public ChemicalAgentService(AgrochemContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }

        public async Task<List<ChemicalAgentDTO>> GetChemicalAgents()
        {
            try
            {
                var chemicalAgents = await _context.ChemicalAgents
                     .Select(c => new ChemicalAgentDTO
                     {
                         ChemAgentId=c.ChemAgentId,
                         Name=c.Name,
                         Type=c.Type,
                         Description=c.Description,
                         Archival=c.Archival                      
                     })
                    .ToListAsync();
                return chemicalAgents;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                throw new ApplicationException("Błąd podczas pobierania działek", ex);
            }
        }

        public async Task<string> AddChemicalAgent(ChemicalAgentDTO chemicalAgentDTO)
        {
            try
            {
                var chemAgent = _context.ChemicalAgents
                .FirstOrDefault(p => p.Name == chemicalAgentDTO.Name);
                if (chemAgent != null)
                {
                    return "Środek chemiczny o tej nazwie juz istnieje.";
                }


                var newChemAgent = new ChemicalAgent
                {
                    Name = chemicalAgentDTO.Name,
                    Type = chemicalAgentDTO.Type,
                    Description = chemicalAgentDTO.Description,
                    Archival=false
                };

                _context.ChemicalAgents.Add(newChemAgent);
                await _context.SaveChangesAsync();
                return "Utworzono środek chemiczny.";
            }
            catch (Exception ex)
            {
                // Logowanie błędu
                Console.WriteLine(ex.Message);
                return ex.Message;
            }
        }

        public async Task<bool> UpdateArchiveChemAgent(int id, bool archive)
        {
            try
            {
                var chemAgent = await _context.ChemicalAgents.FindAsync(id);
                if (chemAgent == null)
                {
                    return false;
                }

                chemAgent.Archival = archive;

                _context.ChemicalAgents.Update(chemAgent);
                await _context.SaveChangesAsync();

                return true; // Operacja zakończona sukcesem
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                throw new ApplicationException("Błąd podczas archiwizacj działek", ex);
            }
        }

        public async Task<bool> UpdateChemicalAgent(int id, ChemicalAgentDTO chemicalAgentDTO)
        {
            var chemAgent = await _context.ChemicalAgents.FindAsync(id);
            if (chemAgent == null)
            {
                return false;
            }

            chemAgent.Name = chemicalAgentDTO.Name;
            chemAgent.Type = chemicalAgentDTO.Type;
            chemAgent.Description = chemicalAgentDTO.Description;
            
            _context.ChemicalAgents.Update(chemAgent);
            await _context.SaveChangesAsync();

            return true; // Operacja zakończona sukcesem
        }

    }
}
