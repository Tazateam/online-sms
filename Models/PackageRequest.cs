using System;
using System.Collections.Generic;

namespace online_sms.Models;

public partial class PackageRequest
{
    public int RequestId { get; set; }

    public int UserId { get; set; }

    public string PackageType { get; set; } = null!;

    public DateTime RequestDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
