using System;
using System.Collections.Generic;

namespace online_sms.Models;

public partial class Contact
{
    public int Contactid { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string ContactNumber { get; set; } = null!;

    public int? Userid { get; set; }

    public virtual User? User { get; set; }
}
