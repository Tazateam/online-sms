using System;
using System.Collections.Generic;

namespace online_sms.Models;

public partial class Message
{
    public int MessageId { get; set; }

    public int? SenderUserId { get; set; }

    public int? ReceiverUserId { get; set; }

    public string? ReceiverContactNumber { get; set; }

    public string? MessageText { get; set; }

    public DateTime? SentAt { get; set; }

    public virtual User? SenderUser { get; set; }
}
