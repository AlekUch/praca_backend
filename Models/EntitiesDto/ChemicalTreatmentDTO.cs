namespace AGROCHEM.Models.EntitiesDto
{
    public class ChemicalTreatmentDTO
    {
        public int ChemTreatId { get; set; }

       public int? CultivationId { get; set; }
        public DateTime? Date { get; set; }

        public decimal? Area { get; set; }

        public int? ChemAgentId { get; set; }

        public decimal? Dose { get; set; }

        public string? Reason { get; set; }

    }
}
