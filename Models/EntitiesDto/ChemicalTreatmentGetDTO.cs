using AGROCHEM.Models.Entities;

namespace AGROCHEM.Models.EntitiesDto
{
    public class ChemicalTreatmentGetDTO
    {
        public int ChemTreatId { get; set; }
        public int? CultivationId { get; set; }
        public string PlotNumber { get; set; }
        public string PlantName { get; set; }
        public DateTime? Date { get; set; }

        public decimal? Area { get; set; }

        public int? ChemAgentId { get; set; }
        public string? ChemAgentName { get; set; }

        public decimal? Dose { get; set; }
        public decimal? MaxArea { get; set; }
        public string? Reason { get; set; }
        public int PlantId { get; set; }
      public decimal? MinDose { get; set; }
        public decimal? MaxDose { get; set; }
        public int ChemUseId { get; set; }
    }
}
