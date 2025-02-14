namespace AGROCHEM.Models.EntitiesDto
{
    public class NotificationDTO
    {
        public int NotificationId { get; set; }

        public string ChemAgentName { get; set; }
        public string PlotNumber { get; set; }
        public string PlantName { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool? IsRead { get; set; }
    }
}
