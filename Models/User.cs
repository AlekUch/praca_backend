using System;
using System.Collections.Generic;

namespace AGROCHEM.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public int? RoleId { get; set; }

    public virtual ICollection<Plot> Plots { get; set; } = new List<Plot>();

    public virtual Role? Role { get; set; }
}
