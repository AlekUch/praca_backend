namespace AGROCHEM.Models.EntitiesDto
{
    public class CultivationDTO
    {
        public int CultivationId { get; set; }
        public string PlantName { get; set; }
        public int? PlantId { get; set; }
        public int? PlotId { get; set; }
        public string PlotNumber { get; set; }
        public DateTime? SowingDate { get; set; }

        public DateTime? HarvestDate { get; set; }

        public decimal? Area { get; set; }

        public bool? Archival { get; set; }
    }
}
