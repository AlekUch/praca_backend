using System;
using System.Collections.Generic;

namespace AGROCHEM.Models.Entities;

public partial class PlantDisease
{
    public int PlDiseId { get; set; }

    public int? DiseaseId { get; set; }

    public int? PlantId { get; set; }

    public virtual Disease? Disease { get; set; }

    public virtual Plant? Plant { get; set; }
}
