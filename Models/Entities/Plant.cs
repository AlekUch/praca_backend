using System;
using System.Collections.Generic;

namespace AGROCHEM.Models.Entities;

public partial class Plant
{
    public int PlantId { get; set; }

    public string? Name { get; set; }

    public int? RotationPeriod { get; set; }

    public virtual ICollection<ChemicalTreatment> ChemicalTreatments { get; set; } = new List<ChemicalTreatment>();

    public virtual ICollection<ChemicalUse> ChemicalUses { get; set; } = new List<ChemicalUse>();

    public virtual ICollection<Cultivation> Cultivations { get; set; } = new List<Cultivation>();

    public virtual ICollection<PlantDisease> PlantDiseases { get; set; } = new List<PlantDisease>();
}
