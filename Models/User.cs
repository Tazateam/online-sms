using System;
using System.Collections.Generic;

namespace online_sms.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int MobileNumber { get; set; }

    public virtual UserProfile? UserProfile { get; set; }
}
