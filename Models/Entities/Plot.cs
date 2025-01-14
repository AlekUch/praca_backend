using System;
using System.Collections.Generic;

namespace AGROCHEM.Models.Entities;

public partial class Plot
{
    public int PlotId { get; set; }

    public string? PlotNumber { get; set; }

    public decimal? Area { get; set; }

    public int? OwnerId { get; set; }

    public int? AddressId { get; set; }

    public bool? Archival { get; set; }

    public virtual PlotAddress? Address { get; set; }

   

    public virtual ICollection<Cultivation> Cultivations { get; set; } = new List<Cultivation>();

    public virtual User? Owner { get; set; }
}
