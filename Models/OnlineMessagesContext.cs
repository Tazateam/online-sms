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

    public virtual DbSet<ContactU> ContactUs { get; set; }

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
            entity.HasKey(e => e.ContactId).HasName("PK__Contacts__5C6625BBD2DD7AC6");

            entity.Property(e => e.ContactId).HasColumnName("ContactID");
            entity.Property(e => e.ContactNumber).HasMaxLength(15);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Contacts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Contacts__UserID__5DCAEF64");
        });

        modelBuilder.Entity<ContactU>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ContactU__3214EC074511CF11");

            entity.ToTable("ContactU");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Message)
                .HasMaxLength(250)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Friend>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Friends__3214EC279507B783");

            entity.HasIndex(e => new { e.UserId, e.FriendUserId }, "UQ_UserFriend").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.FriendUserId).HasColumnName("FriendUserID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("('Pending')");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.FriendUser).WithMany(p => p.FriendFriendUsers)
                .HasForeignKey(d => d.FriendUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Friends_FriendUserId");

            entity.HasOne(d => d.User).WithMany(p => p.FriendUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Friends_UserId");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Messages__C87C037CCE1645D6");

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
                .HasConstraintName("FK__Messages__Sender__60A75C0F");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Services__C51BB0EA35CF2A4E");

            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ServiceDescription).HasMaxLength(255);
            entity.Property(e => e.ServiceName).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACECAC3EF0");

            entity.HasIndex(e => e.MobileNumber, "UQ__Users__250375B190E366FE").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E49ECA064B").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534B45CE74E").IsUnique();

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
            entity.Property(e => e.MobileNumber).HasMaxLength(15);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Qualification).HasMaxLength(255);
            entity.Property(e => e.Sports).HasMaxLength(255);
            entity.Property(e => e.Username).HasMaxLength(50);
            entity.Property(e => e.VerificationCode).HasMaxLength(10);
        });

        modelBuilder.Entity<UserService>(entity =>
        {
            entity.HasKey(e => e.UserServiceId).HasName("PK__UserServ__C737CAF9FAF6053D");

            entity.Property(e => e.UserServiceId).HasColumnName("UserServiceID");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.SubscribedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Service).WithMany(p => p.UserServices)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK__UserServi__Servi__619B8048");

            entity.HasOne(d => d.User).WithMany(p => p.UserServices)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserServi__UserI__628FA481");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
