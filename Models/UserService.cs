using System;
using System.Collections.Generic;

namespace online_sms.Models;

public partial class UserService
{
    public int UserServiceId { get; set; }

    public int? UserId { get; set; }

    public int? ServiceId { get; set; }

    public DateTime? SubscribedAt { get; set; }

    public virtual Service? Service { get; set; }

    public virtual User? User { get; set; }
}
