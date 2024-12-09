using System;
using System.Collections.Generic;

namespace AGROCHEM.Models.Entities;

public partial class ChemicalAgent
{
    public int ChemAgentId { get; set; }

    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? Description { get; set; }
    public bool? Archival { get; set; }

    public virtual ICollection<ChemicalTreatment> ChemicalTreatments { get; set; } = new List<ChemicalTreatment>();

    public virtual ICollection<ChemicalUse> ChemicalUses { get; set; } = new List<ChemicalUse>();
}
