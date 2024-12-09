namespace AGROCHEM.Models.EntitiesDto
{
    public class PlantDTO
    {
        public int PlantId { get; set; }

        public string? Name { get; set; }

        public int? RotationPeriod { get; set; }
        public bool? Archival { get; set; }
    }
}
