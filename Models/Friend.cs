using System;
using System.Collections.Generic;

namespace online_sms.Models;

public partial class Friend
{
    public int UserId { get; set; }

    public int FriendUserId { get; set; }

    public string? Status { get; set; }

    public virtual User FriendUser { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
