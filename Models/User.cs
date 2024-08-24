using System;
using System.Collections.Generic;

namespace online_sms.Models;

public partial class User
{
    public int Userid { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string MobileNumber { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Gender { get; set; }

    public DateTime? Dob { get; set; }

    public string? Address { get; set; }

    public string? Hobbies { get; set; }

    public string? Bio { get; set; }

    public string? MaritalStatus { get; set; }

    public string? Likes { get; set; }

    public string? Dislikes { get; set; }

    public string? Cuisines { get; set; }

    public string? Sports { get; set; }

    public byte[]? ProfilePhoto { get; set; }

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}
