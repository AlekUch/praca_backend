namespace AGROCHEM.Models.Entities
{
    public class Notification
    {
        public int NotificationId { get; set; }

        public int? UserId { get; set; }

        public int? ChemAgentId { get; set; }

        public int? CultivationId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool? IsRead { get; set; }

        public virtual ChemicalAgent? ChemAgent { get; set; }

        public virtual Cultivation? Cultivation { get; set; }

        public virtual User? User { get; set; }
    }

}
