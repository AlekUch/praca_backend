namespace AGROCHEM.Models.EntitiesDto
{
    public class DiseasePhotoDTO
    {
        public string? Characteristic { get; set; }
        public string? Name { get; set; }
        public string? Reasons { get; set; }
        public IFormFile? File { get; set; }
        public string? Prevention { get; set; }
        public List<int>? PlantDisease {  get; set; }
    }
}
