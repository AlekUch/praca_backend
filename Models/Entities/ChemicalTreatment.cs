using System;
using System.Collections.Generic;

namespace AGROCHEM.Models.Entities;

public partial class ChemicalTreatment
{
    public int ChemTreatId { get; set; }

    public int? CultivationId { get; set; }

    public DateTime? Date { get; set; }

    public decimal? Area { get; set; }

    public int? ChemAgentId { get; set; }

    public decimal? Dose { get; set; }

    public string? Reason { get; set; }

    public virtual ChemicalAgent? ChemAgent { get; set; }

    public virtual Cultivation? Cultivation { get; set; }

}
