using System;
using System.Collections.Generic;

namespace online_sms.Models;

public partial class Package
{
    public int PackageId { get; set; }

    public string? PackageName { get; set; }

    public decimal? PackagePrice { get; set; }

    public int? MessageCount { get; set; }

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}
