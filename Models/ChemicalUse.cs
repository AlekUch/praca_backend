using System;
using System.Collections.Generic;

namespace AGROCHEM.Models;

public partial class ChemicalUse
{
    public int ChemUseId { get; set; }

    public int? ChemAgentId { get; set; }

    public int? PlantId { get; set; }

    public decimal? MinDose { get; set; }

    public decimal? MaxDose { get; set; }

    public int? MinWater { get; set; }

    public int? MaxWater { get; set; }

    public int? MinDays { get; set; }

    public int? MaxDays { get; set; }

    public virtual ChemicalAgent? ChemAgent { get; set; }

    public virtual Plant? Plant { get; set; }
}
