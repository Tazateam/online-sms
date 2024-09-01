using System;
using System.Collections.Generic;

namespace online_sms.Models;

public partial class ContactU
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string ContactNumber { get; set; } = null!;

    public string Message { get; set; } = null!;
}
