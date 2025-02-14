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

        public async Task<List<ChemicalAgentDTO>> GetChemicalAgents(bool isArchive)
        {
            try
            {
                if (isArchive == false)
                {
                    var chemicalAgents = await _context.ChemicalAgents
                        .Where(c => c.Archival == isArchive)
                        .Include(p => p.Photo)
                         .Select(c => new ChemicalAgentDTO
                         {
                             ChemAgentId = c.ChemAgentId,
                             Name = c.Name,
                             Type = c.Type,
                             Description = c.Description,
                             Archival = c.Archival,
                             PhotoName = c.Photo.Name,
                             Photo = c.Photo != null
                               ? $"data:{c.Photo.Type};base64,{Convert.ToBase64String(c.Photo.BinaryData)}"
                               : null,
                         })
                        .ToListAsync();
                    return chemicalAgents;
                }
                else
                {
                    var chemicalAgents = await _context.ChemicalAgents
                        .Include(p => p.Photo)
                         .Select(c => new ChemicalAgentDTO
                         {
                             ChemAgentId = c.ChemAgentId,
                             Name = c.Name,
                             Type = c.Type,
                             Description = c.Description,
                             Archival = c.Archival,
                             PhotoName = c.Photo.Name,
                             Photo = c.Photo != null
                               ? $"data:{c.Photo.Type};base64,{Convert.ToBase64String(c.Photo.BinaryData)}"
                               : null,
                         })
                        .ToListAsync();
                    return chemicalAgents;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                throw new ApplicationException("Błąd podczas pobierania działek", ex);
            }
        }

        public async Task<List<ChemicalAgentDTO>> GetChemicalAgentsForUser()
        {
            try
            {
                var chemicalAgents = await _context.ChemicalAgents
                     .Where(c=>c.Archival==false)
                     .Select(c => new ChemicalAgentDTO
                     {
                         ChemAgentId = c.ChemAgentId,
                         Name = c.Name,
                         Type = c.Type,
                         Description = c.Description,
                         Archival = c.Archival
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

        public async Task<ChemicalAgentDTO> GetChemicalAgentById(int id)
        {
            try
            {
                var chemicalAgents = await _context.ChemicalAgents
                    .Include(p => p.Photo)
                    .Where(p=>p.ChemAgentId==id)
                     .Select(c => new ChemicalAgentDTO
                     {
                         ChemAgentId = c.ChemAgentId,
                         Name = c.Name,
                         Type = c.Type,
                         Description = c.Description,
                         Archival = c.Archival,
                         PhotoName = c.Photo.Name,
                         Photo = c.Photo != null
                           ? $"data:{c.Photo.Type};base64,{Convert.ToBase64String(c.Photo.BinaryData)}"
                           : null,
                     })
                    .FirstOrDefaultAsync();
                return chemicalAgents;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                throw new ApplicationException("Błąd podczas pobierania działek", ex);
            }
        }

        public async Task<string> AddChemicalAgent(ChemicalAgentPhotoDTO chemicalAgentPhotoDTO)
        {
            
                var chemAgent = _context.ChemicalAgents
                .FirstOrDefault(p => p.Name == chemicalAgentPhotoDTO.Name);
                if (chemAgent != null)
                {
                    return "Środek chemiczny o tej nazwie juz istnieje.";
                }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {                
                using var memoryStream = new MemoryStream();
                await chemicalAgentPhotoDTO.File.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();

                var photo = new Photo
                {
                    BinaryData = fileBytes,
                    Name = chemicalAgentPhotoDTO.File.FileName,
                    Extension = Path.GetExtension(chemicalAgentPhotoDTO.File.FileName),
                    Type = chemicalAgentPhotoDTO.File.ContentType
                };

                _context.Photos.Add(photo);
                await _context.SaveChangesAsync();

                var newChemAgent = new ChemicalAgent
                {
                    Name = chemicalAgentPhotoDTO.Name,
                    Type = chemicalAgentPhotoDTO.Type,
                    Description = chemicalAgentPhotoDTO.Description,
                    Archival=false,
                    PhotoId = photo.PhotoId,
                };

                _context.ChemicalAgents.Add(newChemAgent);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return "Utworzono środek chemiczny.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
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

        public async Task<bool> UpdateChemicalAgent(int id, ChemicalAgentPhotoDTO chemicalAgentPhotoDTO)
        {

            var chemAgent = await _context.ChemicalAgents.FindAsync(id);
            if (chemAgent == null)
            {
                return false;
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try {
                chemAgent.Name = chemicalAgentPhotoDTO.Name;
                chemAgent.Type = chemicalAgentPhotoDTO.Type;
                chemAgent.Description = chemicalAgentPhotoDTO.Description;

                if (chemicalAgentPhotoDTO.File != null)
                {
                    var photoId = chemAgent.PhotoId;
                    var photoToDelete = await _context.Photos.FindAsync(photoId);
                    if (photoToDelete != null) 
                    {
                        _context.Photos.Remove(photoToDelete);
                    }

                    using var memoryStream = new MemoryStream();
                    await chemicalAgentPhotoDTO.File.CopyToAsync(memoryStream);
                    var fileBytes = memoryStream.ToArray();

                    var photo = new Photo
                    {
                        BinaryData = fileBytes,
                        Name = chemicalAgentPhotoDTO.File.FileName,
                        Extension = Path.GetExtension(chemicalAgentPhotoDTO.File.FileName),
                        Type = chemicalAgentPhotoDTO.File.ContentType
                    };

                    _context.Photos.Add(photo);
                    await _context.SaveChangesAsync();
                    chemAgent.PhotoId = photo.PhotoId;
                }
                _context.ChemicalAgents.Update(chemAgent);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true; 
            
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await transaction.RollbackAsync();
                return false;
            }
        }

    }
}
