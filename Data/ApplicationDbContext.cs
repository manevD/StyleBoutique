using BoutiqueQuantity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BoutiqueQuantity.Data
{
    public class ApplicationDbContext
       : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<InventoryLog> InventoryLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProductVariant>(entity =>
            {
                entity.HasOne(x => x.Product)
                    .WithMany(x => x.Variants)
                    .HasForeignKey(x => x.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.HasOne(x => x.ProductVariant)
                    .WithMany(x => x.Inventories)
                    .HasForeignKey(x => x.ProductVariantId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(x => new
                {
                    x.ProductVariantId,
                    x.Size
                }).IsUnique();
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasOne(x => x.ProductVariant)
                    .WithMany()
                    .HasForeignKey(x => x.ProductVariantId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<InventoryLog>(entity =>
            {
                entity.HasOne(x => x.ProductVariant)
                    .WithMany()
                    .HasForeignKey(x => x.ProductVariantId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(x => x.ProductCode)
                    .IsUnique();
            });

            modelBuilder.Entity<Sale>()
                .HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Sale>()
                .HasOne(x => x.ProductVariant)
                .WithMany()
                .HasForeignKey(x => x.ProductVariantId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Sale>()
                .HasOne(x => x.Inventory)
                .WithMany()
                .HasForeignKey(x => x.InventoryId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Product>()
.HasOne(x => x.Category)
.WithMany()
.HasForeignKey(x => x.CategoryId)
.OnDelete(DeleteBehavior.Restrict);
        }
    }
}
