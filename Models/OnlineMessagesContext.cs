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

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<ContactU> ContactUs { get; set; }

    public virtual DbSet<Friend> Friends { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserService> UserServices { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("data source=.;initial catalog=Online_Messages;user id=sa;password=aptech; TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__Admin__719FE4E8F5AE691E");

            entity.ToTable("Admin");

            entity.Property(e => e.AdminId).HasColumnName("AdminID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(50);
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.ContactId).HasName("PK__Contacts__5C6625BBB6953286");

            entity.Property(e => e.ContactId).HasColumnName("ContactID");
            entity.Property(e => e.ContactNumber).HasMaxLength(15);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Contacts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Contacts__UserID__3A81B327");
        });

        modelBuilder.Entity<ContactU>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ContactU__3214EC0735077AF5");

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
            entity.HasKey(e => e.Id).HasName("PK__Friends__3214EC27DBB77F0E");

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
            entity.HasKey(e => e.MessageId).HasName("PK__Messages__C87C037C844F1AB0");

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
                .HasConstraintName("FK__Messages__Sender__3D5E1FD2");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Services__C51BB0EA301877F6");

            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ServiceDescription).HasMaxLength(255);
            entity.Property(e => e.ServiceName).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC7EDE7D8B");

            entity.HasIndex(e => e.MobileNumber, "UQ__Users__250375B1B530B6F3").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E415B8EF06").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534BF9F4AA4").IsUnique();

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
            entity.Property(e => e.MsgCount).HasDefaultValueSql("((2))");
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Qualification).HasMaxLength(255);
            entity.Property(e => e.Sports).HasMaxLength(255);
            entity.Property(e => e.Username).HasMaxLength(50);
            entity.Property(e => e.VerificationCode).HasMaxLength(10);
        });

        modelBuilder.Entity<UserService>(entity =>
        {
            entity.HasKey(e => e.UserServiceId).HasName("PK__UserServ__C737CAF9B171A380");

            entity.Property(e => e.UserServiceId).HasColumnName("UserServiceID");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.SubscribedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Service).WithMany(p => p.UserServices)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK__UserServi__Servi__3E52440B");

            entity.HasOne(d => d.User).WithMany(p => p.UserServices)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserServi__UserI__3F466844");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
