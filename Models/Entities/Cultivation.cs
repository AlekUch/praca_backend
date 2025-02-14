using System;
using System.Collections.Generic;

namespace AGROCHEM.Models.Entities;

public partial class Cultivation
{
    public int CultivationId { get; set; }

    public int? PlotId { get; set; }

    public int? PlantId { get; set; }

    public DateTime? SowingDate { get; set; }

    public DateTime? HarvestDate { get; set; }

    public decimal? Area { get; set; }

    public bool? Archival { get; set; }

    public virtual Plant? Plant { get; set; }

    public virtual Plot? Plot { get; set; }
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public virtual ICollection<ChemicalTreatment> ChemicalTreatments { get; set; } = new List<ChemicalTreatment>();
}
