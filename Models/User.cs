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

    public string? VerificationCode { get; set; }

    public bool? IsVerified { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

    public virtual ICollection<Friend> FriendFriendUsers { get; set; } = new List<Friend>();

    public virtual ICollection<Friend> FriendUsers { get; set; } = new List<Friend>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual UserProfile? UserProfile { get; set; }

    public virtual ICollection<UserService> UserServices { get; set; } = new List<UserService>();
}
