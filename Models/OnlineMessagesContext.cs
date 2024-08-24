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

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("data source=.;initial catalog=Online_Messages;user id=sa;password=aptech; TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.Contactid).HasName("PK__Contacts__5C6521B3A448FF23");

            entity.HasIndex(e => e.ContactNumber, "UQ__Contacts__570665C6A78F43E5").IsUnique();

            entity.Property(e => e.ContactNumber).HasMaxLength(10);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.Contacts)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("FK__Contacts__Userid__3D5E1FD2");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("PK__Users__1797D0241CBEC52B");

            entity.HasIndex(e => e.MobileNumber, "UQ__Users__250375B1F5DF0BD0").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E469B73161").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534EC3A75B9").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.Bio).HasMaxLength(100);
            entity.Property(e => e.Cuisines).HasMaxLength(255);
            entity.Property(e => e.Dislikes).HasMaxLength(255);
            entity.Property(e => e.Dob)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.Gender).HasMaxLength(50);
            entity.Property(e => e.Hobbies).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Likes).HasMaxLength(255);
            entity.Property(e => e.MaritalStatus).HasMaxLength(20);
            entity.Property(e => e.MobileNumber).HasMaxLength(10);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Sports).HasMaxLength(255);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
