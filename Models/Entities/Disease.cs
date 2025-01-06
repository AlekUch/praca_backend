using System;
using System.Collections.Generic;

namespace AGROCHEM.Models.Entities;

public partial class Disease
{
    public int DiseaseId { get; set; }

    public string Name { get; set; }
    public int? PhotoId { get; set; }

    public string? Characteristic { get; set; }

    public string? Reasons { get; set; }
    public string? Prevention { get; set; }

    public virtual Photo? Photo { get; set; }

    public virtual ICollection<PlantDisease> PlantDiseases { get; set; } = new List<PlantDisease>();
}
