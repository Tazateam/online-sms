using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace online_sms.Models;

public partial class OnlineMessagesContext : DbContext
{
    public OnlineMessagesContext()
    {
    }

    public OnlineMessagesContext(DbContextOptions<OnlineMessagesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<Friend> Friends { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserService> UserServices { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("data source=DESKTOP-DIBKE47\\SQLEXPRESS;initial catalog=Online_Messages;user id=sa;password=anas; TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.ContactId).HasName("PK__Contacts__5C6625BBDD58E7F5");

            entity.Property(e => e.ContactId).HasColumnName("ContactID");
            entity.Property(e => e.ContactNumber).HasMaxLength(15);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Contacts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Contacts__UserID__5AEE82B9");
        });

        modelBuilder.Entity<Friend>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.FriendUserId }).HasName("PK__Friends__11BD2B9CE2E587AC");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.FriendUserId).HasColumnName("FriendUserID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("('Pending')");

            entity.HasOne(d => d.FriendUser).WithMany(p => p.FriendFriendUsers)
                .HasForeignKey(d => d.FriendUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Friends__FriendU__5CD6CB2B");

            entity.HasOne(d => d.User).WithMany(p => p.FriendUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Friends__UserID__5BE2A6F2");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Messages__C87C037CA9948945");

            entity.Property(e => e.MessageId).HasColumnName("MessageID");
            entity.Property(e => e.MessageText).HasMaxLength(120);
            entity.Property(e => e.ReceiverContactNumber).HasMaxLength(10);
            entity.Property(e => e.ReceiverUserId).HasColumnName("ReceiverUserID");
            entity.Property(e => e.SenderUserId).HasColumnName("SenderUserID");
            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.SenderUser).WithMany(p => p.Messages)
                .HasForeignKey(d => d.SenderUserId)
                .HasConstraintName("FK__Messages__Sender__5DCAEF64");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Services__C51BB0EAF05A1494");

            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ServiceDescription).HasMaxLength(255);
            entity.Property(e => e.ServiceName).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACE7CE7584");

            entity.HasIndex(e => e.MobileNumber, "UQ__Users__250375B1869E0F5B").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E46CBA5994").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D1053457A010B3").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Designation).HasMaxLength(255);
            entity.Property(e => e.Dob)
                .HasColumnType("date")
                .HasColumnName("DOB");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.Hobbies).HasMaxLength(255);
            entity.Property(e => e.IsVerified).HasDefaultValueSql("((0))");
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.MaritalStatus).HasMaxLength(20);
            entity.Property(e => e.MobileNumber).HasMaxLength(10);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Qualification).HasMaxLength(255);
            entity.Property(e => e.Sports).HasMaxLength(255);
            entity.Property(e => e.Username).HasMaxLength(50);
            entity.Property(e => e.VerificationCode).HasMaxLength(10);
        });

        modelBuilder.Entity<UserService>(entity =>
        {
            entity.HasKey(e => e.UserServiceId).HasName("PK__UserServ__C737CAF967421642");

            entity.Property(e => e.UserServiceId).HasColumnName("UserServiceID");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.SubscribedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Service).WithMany(p => p.UserServices)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK__UserServi__Servi__5FB337D6");

            entity.HasOne(d => d.User).WithMany(p => p.UserServices)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserServi__UserI__5EBF139D");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
