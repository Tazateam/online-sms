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

    public int MsgCount { get; set; }

    public string? VerificationCode { get; set; }

    public bool? IsVerified { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Gender { get; set; }

    public DateTime? Dob { get; set; }

    public string? Address { get; set; }

    public string? MaritalStatus { get; set; }

    public string? Hobbies { get; set; }

    public string? Sports { get; set; }

    public string? ProfilePhoto { get; set; }

    public string? Qualification { get; set; }

    public string? Designation { get; set; }

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

    public virtual ICollection<Friend> FriendFriendUsers { get; set; } = new List<Friend>();

    public virtual ICollection<Friend> FriendUsers { get; set; } = new List<Friend>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<UserService> UserServices { get; set; } = new List<UserService>();
}
