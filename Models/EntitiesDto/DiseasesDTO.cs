namespace AGROCHEM.Models.EntitiesDto
{
    public class DiseasesDTO
    {
        public int DiseaseId { get; set; }

        public string Name { get; set; }
        public string? Characteristic { get; set; }

        public string? Reasons { get; set; }
        public string? Photo {  get; set; }
        public string? Prevention { get; set; }
        public string PhotoName { get; set; }
        public string PlantDiseases { get; set; }
        public string PlantsId { get; set; }
    }
}
