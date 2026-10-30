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
        public DbSet<Color> Color => Set<Color>();
        public DbSet<ProductOption> ProductOptions => Set<ProductOption>();
        public DbSet<DataSource> DataSources => Set<DataSource>();
        public DbSet<ProductDataSource> ProductDataSources => Set<ProductDataSource>();        
        public DbSet<Marketplace> Marketplaces => Set<Marketplace>();
        public DbSet<ProductMarketplace> ProductMarketplaces => Set<ProductMarketplace>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
        public DbSet<SizeGroup> SizeGroups => Set<SizeGroup>();
        public DbSet<Size> Sizes => Set<Size>();
        public DbSet<ProductSize> ProductSizes => Set<ProductSize>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Sku).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
                entity.Property(p => p.Embedding).HasColumnType("vector(1536)");
                entity.HasIndex(e => e.Sku).IsUnique();   // SKU único blindado en BBDD

                

                // Configuramos la relación desde aquí, ya que ProductOption ya no tiene navegación a Product.
                // "WithOne()" sin argumentos le dice a EF: "el otro lado no tiene propiedad de navegación".
                entity.HasMany(e => e.ProductOptions)
                      .WithOne()
                      .HasForeignKey(po => po.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
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

            modelBuilder.Entity<Color>(entity =>
            {
                entity.ToTable("Colors");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).HasMaxLength(50).IsRequired();
                entity.Property(c => c.HexCode).HasMaxLength(9).IsRequired();
                entity.Property(c => c.Embedding).HasColumnType("vector(1536)");
                entity.HasIndex(c => c.Name).IsUnique();               
            });

            //tabla intermedia con colores, nombre equivocado, pero es la que hay
            modelBuilder.Entity<ProductOption>(entity =>
            {
                entity.ToTable("ProductOptions");
                entity.HasKey(po => po.Id);
                entity.HasIndex(po => new { po.ProductId, po.ColorId }).IsUnique();

                // La relación con Color SÍ la configuramos aquí, porque ProductOption SÍ tiene 
                // la propiedad de navegación 'Color' (la que añadimos para que funcione el Include).
                entity.HasOne(po => po.Color)
                      .WithMany()
                      .HasForeignKey(po => po.ColorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });


            // Configuración de Category
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            // Configuración de ProductCategory (Tabla de unión N:M)
            modelBuilder.Entity<ProductCategory>(entity =>
            {
                // Clave compuesta
                entity.HasKey(e => new { e.ProductId, e.CategoryId });

                // Relación con Product
                entity.HasOne(e => e.Product)
                      .WithMany(p => p.ProductCategories)
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relación con Category
                entity.HasOne(e => e.Category)
                      .WithMany(c => c.ProductCategories)
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de SizeGroup
            modelBuilder.Entity<SizeGroup>(entity =>
            {
                entity.ToTable("SizeGroups"); // <-- AÑADIDO: Nombre explícito
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });
            //configuración de Size
            modelBuilder.Entity<Size>(entity =>
            {
                entity.ToTable("Sizes");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                // Relación con SizeGroup
                entity.HasOne(e => e.SizeGroup)
                      .WithMany(sg => sg.Sizes)
                      .HasForeignKey(e => e.SizeGroupId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            // Configuración de ProductSize (Tabla de unión N:M con payload)
            modelBuilder.Entity<ProductSize>(entity =>
            {
                entity.ToTable("ProductSizes");
                // Clave compuesta
                entity.HasKey(e => new { e.ProductId, e.SizeId });
                // Relación con Product
                entity.HasOne(e => e.Product)
                      .WithMany(p => p.ProductSizes)
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
                // Relación con Size
                entity.HasOne(e => e.Size)
                      .WithMany(s => s.ProductSizes)
                      .HasForeignKey(e => e.SizeId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de ProductDatasource (Tabla de unión N:M)
            modelBuilder.Entity<ProductDataSource>(entity =>
            {
                // Clave compuesta
                entity.HasKey(e => new { e.ProductId, e.DataSourceId });

                // Relación con Product
                entity.HasOne(e => e.Product)
                      .WithMany(p => p.ProductDataSources)
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relación con DataSource
                entity.HasOne(e => e.DataSource)
                      .WithMany(c => c.ProductDataSources)
                      .HasForeignKey(e => e.DataSourceId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de ProductMarketplace (Tabla de unión N:M)
            modelBuilder.Entity<ProductMarketplace>(entity =>
            {
                // Clave compuesta
                entity.HasKey(e => new { e.ProductId, e.MarketplaceId });

                // Relación con Product
                entity.HasOne(e => e.Product)
                      .WithMany(p => p.ProductMarketplaces)
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relación con Marketplace
                entity.HasOne(e => e.Marketplace)
                      .WithMany(m => m.ProductMarketplaces)
                      .HasForeignKey(e => e.MarketplaceId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

        }
    }
}
