using System;
using System.Collections.Generic;

namespace AGROCHEM.Models;

public partial class ChemicalAgent
{
    public int ChemAgentId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<ChemicalTreatment> ChemicalTreatments { get; set; } = new List<ChemicalTreatment>();

    public virtual ICollection<ChemicalUse> ChemicalUses { get; set; } = new List<ChemicalUse>();
}
