using System;
using System.Collections.Generic;

namespace AGROCHEM.Models;

public partial class PlotAddress
{
    public int PlotAddressId { get; set; }

    public string? Location { get; set; }

    public string? District { get; set; }

    public string? Voivodeship { get; set; }

    public virtual ICollection<Plot> Plots { get; set; } = new List<Plot>();
}
