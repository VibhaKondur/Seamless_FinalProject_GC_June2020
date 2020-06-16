using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace GC_FinalProject_Seamless_June2020.Models
{
    public partial class SeamedInDBContext : DbContext
    {
        public SeamedInDBContext()
        {
        }

        public SeamedInDBContext(DbContextOptions<SeamedInDBContext> options)
            : base(options)
        {
        }

        public IConfiguration Configuration { get; }

        public virtual DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<Favorites> Favorites { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DBContext"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRoleClaims>(entity =>
            {
                entity.HasIndex(e => e.RoleId);

                entity.Property(e => e.RoleId).IsRequired();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetRoles>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName)
                    .HasName("RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaims>(entity =>
            {
                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogins>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.ProviderKey).HasMaxLength(128);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasIndex(e => e.RoleId);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserTokens>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.Name).HasMaxLength(128);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUsers>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail)
                    .HasName("EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName)
                    .HasName("UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.Roles).HasMaxLength(50);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<Favorites>(entity =>
            {
                entity.Property(e => e.ApiId).HasColumnName("API-Id");

                entity.Property(e => e.UserId).HasMaxLength(450);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Favorites)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Favorites__UserI__60A75C0F");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.Property(e => e.City)
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EmailAddress)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Industry)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Landscape).HasMaxLength(100);

                entity.Property(e => e.Name).HasMaxLength(75);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.ProfilePicture).HasMaxLength(100);

                entity.Property(e => e.Roles)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.StateProvince)
                    .HasColumnName("State/Province")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Summary).HasMaxLength(400);

                entity.Property(e => e.Technology)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Theme)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.UserId).HasMaxLength(450);

                entity.Property(e => e.UserType)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Website)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Users__UserId__76969D2E");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
