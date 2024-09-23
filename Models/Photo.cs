using System;
using System.Collections.Generic;

namespace AGROCHEM.Models;

public partial class Photo
{
    public int PhotoId { get; set; }

    public string? Name { get; set; }

    public string? Extension { get; set; }

    public string? Type { get; set; }

    public byte[]? BinaryData { get; set; }

    public virtual ICollection<Disease> Diseases { get; set; } = new List<Disease>();
}
