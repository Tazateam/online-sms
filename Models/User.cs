using System;
using System.Collections.Generic;

namespace online_sms.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string MobileNumber { get; set; } = null!;

    public string? Gender { get; set; }

    public DateTime? Dob { get; set; }

    public string? Address { get; set; }

    public string? MaritalStatus { get; set; }

    public string? Hobbies { get; set; }

    public string? Likes { get; set; }

    public string? Dislikes { get; set; }

    public string? Cuisines { get; set; }

    public string? Sports { get; set; }

    public byte[]? ProfilePhoto { get; set; }

    public string? Qualification { get; set; }

    public string? School { get; set; }

    public string? College { get; set; }

    public string? WorkStatus { get; set; }

    public string? Organization { get; set; }

    public string? Designation { get; set; }

    public string? VerificationCode { get; set; }

    public bool? IsVerified { get; set; }

    public DateTime? CreatedAt { get; set; }
}
