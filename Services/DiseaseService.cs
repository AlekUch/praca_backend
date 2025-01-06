using AGROCHEM.Data;
using AGROCHEM.Models.Entities;
using AGROCHEM.Models.EntitiesDto;
using Azure.Core;
using Microsoft.EntityFrameworkCore;

namespace AGROCHEM.Services
{
    public class DiseaseService
    {
        private readonly AgrochemContext _context;
        private readonly IConfiguration _configuration;

        public DiseaseService(AgrochemContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }

        public async Task<List<DiseasesDTO>> GetDisease()
        {
            try
            {
                var diseases = await _context.Diseases
                    .Include(p => p.Photo)
                     .Select(p => new DiseasesDTO
                     {
                         DiseaseId = p.DiseaseId,
                         Name = p.Name,
                         Characteristic = p.Characteristic,
                         Reasons = p.Reasons,
                         Prevention = p.Prevention,
                         PhotoName = p.Photo.Name,
                         Photo = p.Photo != null
                           ? $"data:{p.Photo.Type};base64,{Convert.ToBase64String(p.Photo.BinaryData)}"
                           : null,
                         PlantDiseases = string.Join(", ",
                            p.PlantDiseases
                            .Select(pd => pd.Plant.Name
                         )),
                         PlantsId = string.Join(",",
                          p.PlantDiseases
                            .Select(pd => pd.Plant.PlantId
                         ))
                         
                     })
                    .ToListAsync();
                return diseases;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                throw new ApplicationException("Błąd podczas pobierania chorób", ex);
            }
        }

        public async Task<DiseasesDTO> GetDiseaseById(int id)
        {
            try
            {
                var diseases = await _context.Diseases
                    .Where(p => p.DiseaseId == id)
                     .Select(p => new DiseasesDTO
                     {
                         DiseaseId = p.DiseaseId,
                         Name = p.Name,
                         Characteristic = p.Characteristic,
                         Reasons = p.Reasons,
                         Prevention = p.Prevention,
                         PhotoName = p.Photo.Name,
                         Photo = p.Photo != null
                           ? $"data:{p.Photo.Type};base64,{Convert.ToBase64String(p.Photo.BinaryData)}"
                           : null,
                         PlantDiseases = string.Join(", ",
                            p.PlantDiseases
                            .Select(pd => pd.Plant.Name
                         ))
                     })
                    .FirstOrDefaultAsync();
                return diseases;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                throw new ApplicationException("Błąd podczas pobierania danych", ex);
            }
        }

        public async Task<string> AddDisease(DiseasePhotoDTO diseasePhotoDTO)
        {
            if (diseasePhotoDTO.File == null || diseasePhotoDTO.File.Length == 0)
            {
                return "Nie przesłano pliku.";
            }

            var disease = _context.Diseases
                .FirstOrDefault(p => p.Name == diseasePhotoDTO.Name);
            if (disease != null)
            {
                return "Taka choroba już istnieje.";
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                using var memoryStream = new MemoryStream();
                await diseasePhotoDTO.File.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();

                var photo = new Photo
                {
                    BinaryData = fileBytes,
                    Name = diseasePhotoDTO.File.FileName,
                    Extension = Path.GetExtension(diseasePhotoDTO.File.FileName),
                    Type = diseasePhotoDTO.File.ContentType
                };

                _context.Photos.Add(photo);
                await _context.SaveChangesAsync();

                var newDisease = new Disease
                {
                    Name = diseasePhotoDTO.Name,
                    Characteristic = diseasePhotoDTO.Characteristic,
                    Reasons = diseasePhotoDTO.Reasons,
                    PhotoId = photo.PhotoId,
                    Prevention = diseasePhotoDTO.Prevention
                    // Archival = false
                };

                _context.Diseases.Add(newDisease);
                await _context.SaveChangesAsync();

                if (diseasePhotoDTO.PlantDisease != null && diseasePhotoDTO.PlantDisease.Any())
                {
                    var plantDisease = await _context.Plants
                        .Where(p => diseasePhotoDTO.PlantDisease.Contains(p.PlantId))
                        .ToListAsync();

                    foreach (var plant in plantDisease)
                    {
                        // Tworzenie powiązań
                        var newPlantDisease = new PlantDisease
                        {
                            DiseaseId = newDisease.DiseaseId,
                            PlantId = plant.PlantId
                        };
                        _context.PlantDiseases.Add(newPlantDisease);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return "Utworzono chorobę.";
            }
            catch (Exception ex)
            {
                // Logowanie błędu
                Console.WriteLine(ex.Message);
                await transaction.RollbackAsync();
                return ex.Message;
            }
        }

        public async Task<bool> UpdateDisease(int id, DiseasePhotoDTO diseasePhotoDTO)
        {
            var disease = await _context.Diseases.FindAsync(id);
            if (disease == null)
            {
                return false;
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                disease.Name = diseasePhotoDTO.Name;
            disease.Characteristic = diseasePhotoDTO.Characteristic;
            disease.Reasons = diseasePhotoDTO.Reasons;
            disease.Prevention = diseasePhotoDTO.Prevention;
            if (diseasePhotoDTO.File != null )
            {
                using var memoryStream = new MemoryStream();
                await diseasePhotoDTO.File.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();

                var photo = new Photo
                {
                    BinaryData = fileBytes,
                    Name = diseasePhotoDTO.File.FileName,
                    Extension = Path.GetExtension(diseasePhotoDTO.File.FileName),
                    Type = diseasePhotoDTO.File.ContentType
                };

                _context.Photos.Add(photo);
                await _context.SaveChangesAsync();
                disease.PhotoId = photo.PhotoId;
            }

            _context.Diseases.Update(disease);
            

            var recordsToDelete = _context.PlantDiseases.Where(r => r.DiseaseId == id);

            _context.PlantDiseases.RemoveRange(recordsToDelete);

            

            if (diseasePhotoDTO.PlantDisease != null && diseasePhotoDTO.PlantDisease.Any())
            {
                var plantDisease = await _context.Plants
                    .Where(p => diseasePhotoDTO.PlantDisease.Contains(p.PlantId))
                    .ToListAsync();

                foreach (var plant in plantDisease)
                {
                    var newPlantDisease = new PlantDisease
                    {
                        DiseaseId = id,
                        PlantId = plant.PlantId
                    };
                    _context.PlantDiseases.Add(newPlantDisease);
                }
            }
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true; // Operacja zakończona sukcesem
            }
            catch (Exception ex)
            {
                // Logowanie błędu
                Console.WriteLine(ex.Message);
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> DeleteDisease(int id)
        {
            try
            {
                var disease = await _context.Diseases.FindAsync(id);
                if (disease == null)
                {
                    return false;
                }

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var recordsToDelete = _context.PlantDiseases.Where(r => r.DiseaseId == id);
                    _context.PlantDiseases.RemoveRange(recordsToDelete);

                    var photoId = disease.PhotoId;
                    var photo = await _context.Photos.FindAsync(photoId);
                    if (photo != null) // Sprawdź, czy znaleziono obiekt
                    {
                        _context.Photos.Remove(photo);
                    }
                    _context.Diseases.Remove(disease);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    // Logowanie błędu
                    Console.WriteLine(ex.Message);
                    await transaction.RollbackAsync();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                throw new ApplicationException("Błąd podczas archiwizacj działek", ex);
            }
        }
    }
}
