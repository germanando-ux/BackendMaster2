using BackendMaster2.Shared.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Person> Persons => Set<Person>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Sku).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
                entity.HasIndex(e => e.Sku).IsUnique();   // SKU único blindado en BBDD
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();          // dos cuentas con el mismo email: bug con patas
                entity.Property(u => u.Email).HasMaxLength(250).IsRequired();
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.Role).HasMaxLength(20).IsRequired();
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.HasIndex(p => p.UserId).IsUnique();         // el 1-a-1 se garantiza en BD, no por convenio
                entity.Property(p => p.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(p => p.LastName).HasMaxLength(100).IsRequired();
                entity.Property(p => p.Phone).HasMaxLength(30);
                entity.Property(p => p.Address).HasMaxLength(250);

                entity.HasOne(p => p.User)
                      .WithOne(u => u.Person)
                      .HasForeignKey<Person>(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);           // borrada la cuenta, no queda perfil huérfano
            });
        }
    }
}
