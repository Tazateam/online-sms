using System;
using System.Collections.Generic;

namespace online_sms.Models;

public partial class Contact
{
    public int Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? ContactNumber { get; set; }
}
