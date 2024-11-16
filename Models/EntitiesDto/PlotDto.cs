namespace AGROCHEM.Models.EntitiesDto
{
    public class PlotDto
    {
        public string? PlotNumber { get; set; }

        public decimal? Area { get; set; }

        public int? OwnerId { get; set; }
        public bool? Archival { get; set; }
        public string? Location { get; set; }

        public string? District { get; set; }

        public string? Voivodeship { get; set; }

    }
}
