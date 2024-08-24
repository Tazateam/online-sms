using System;
using System.Collections.Generic;

namespace online_sms.Models;

public partial class UserProfile
{
    public int UserId { get; set; }

    public string? Name { get; set; }

    public string? Gender { get; set; }

    public DateTime? Dob { get; set; }

    public string? Address { get; set; }

    public string? MaritalStatus { get; set; }

    public string? Email { get; set; }

    public string? Hobbies { get; set; }

    public string? Likes { get; set; }

    public string? Dislikes { get; set; }

    public string? Cuisines { get; set; }

    public string? Sports { get; set; }

    public byte[]? ProfilePhoto { get; set; }

    public virtual User User { get; set; } = null!;
}
