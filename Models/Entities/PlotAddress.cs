using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AGROCHEM.Models.Entities;

public partial class PlotAddress
{
    public int PlotAddressId { get; set; }

    public string? Location { get; set; }

    public string? District { get; set; }

    public string? Voivodeship { get; set; }

    [JsonIgnore]
    public virtual ICollection<Plot> Plots { get; set; } = new List<Plot>();
}
