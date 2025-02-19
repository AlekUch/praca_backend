using System;
using System.Collections.Generic;

namespace AGROCHEM.Models.Entities;

public partial class User
{
    public int UserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }
    public bool? EmailConfirmed { get; set; }
    public string? EmailConfirmationToken { get; set; }

    public int? RoleId { get; set; }
    public string? PasswordResetToken { get; set; }


    public virtual ICollection<Plot> Plots { get; set; }
        = new List<Plot>();
    public virtual ICollection<Notification> Notifications { get; set;}
        = new List<Notification>();

    public virtual Role? Role { get; set; }
}
