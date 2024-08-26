using System;
using System.Collections.Generic;

namespace online_sms.Models;

public partial class Service
{
    public int ServiceId { get; set; }

    public string? ServiceName { get; set; }

    public string? ServiceDescription { get; set; }

    public decimal? Price { get; set; }

    public virtual ICollection<UserService> UserServices { get; set; } = new List<UserService>();
}
