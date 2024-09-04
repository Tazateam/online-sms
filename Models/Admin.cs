using System;
using System.Collections.Generic;

namespace online_sms.Models;

public partial class Admin
{
    public int AdminId { get; set; }

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;
}
