using BoluBul.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BoluBul.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<City> Cities { get; set; } = null!;

        public DbSet<District> Districts { get; set; } = null!;

        public DbSet<Neighborhood> Neighborhoods { get; set; } = null!;

        public DbSet<Category> Categories { get; set; } = null!;

        public DbSet<Business> Businesses { get; set; } = null!;

        public DbSet<BusinessImage> BusinessImages { get; set; } = null!;

        public DbSet<BusinessHour> BusinessHours { get; set; } = null!;

        public DbSet<Review> Reviews { get; set; } = null!;

        public DbSet<Favorite> Favorites { get; set; } = null!;

        public DbSet<BusinessStat> BusinessStats { get; set; } = null!;

        public DbSet<ContactMessage> ContactMessages { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<City>(entity =>
            {
                entity.HasIndex(c => c.Slug).IsUnique();

                entity.HasMany(c => c.Districts)
                    .WithOne(d => d.City)
                    .HasForeignKey(d => d.CityId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.Businesses)
                    .WithOne(b => b.City)
                    .HasForeignKey(b => b.CityId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<District>(entity =>
            {
                entity.HasIndex(d => new { d.CityId, d.Slug }).IsUnique();

                entity.HasMany(d => d.Neighborhoods)
                    .WithOne(n => n.District)
                    .HasForeignKey(n => n.DistrictId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(d => d.Businesses)
                    .WithOne(b => b.District)
                    .HasForeignKey(b => b.DistrictId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Neighborhood>(entity =>
            {
                entity.HasIndex(n => new { n.DistrictId, n.Slug }).IsUnique();

                entity.HasMany(n => n.Businesses)
                    .WithOne(b => b.Neighborhood)
                    .HasForeignKey(b => b.NeighborhoodId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(c => c.Slug).IsUnique();

                entity.HasMany(c => c.Businesses)
                    .WithOne(b => b.Category)
                    .HasForeignKey(b => b.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Business>(entity =>
            {
                entity.HasIndex(b => b.Slug).IsUnique();

                entity.Property(b => b.Latitude).HasPrecision(10, 7);
                entity.Property(b => b.Longitude).HasPrecision(10, 7);
                entity.Property(b => b.AverageRating).HasPrecision(3, 2);

                entity.HasOne(b => b.Owner)
                    .WithMany(u => u.OwnedBusinesses)
                    .HasForeignKey(b => b.OwnerId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(b => b.Images)
                    .WithOne(i => i.Business)
                    .HasForeignKey(i => i.BusinessId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(b => b.Hours)
                    .WithOne(h => h.Business)
                    .HasForeignKey(h => h.BusinessId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(b => b.Stats)
                    .WithOne(s => s.Business)
                    .HasForeignKey<BusinessStat>(s => s.BusinessId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasIndex(r => new { r.UserId, r.BusinessId }).IsUnique();

                entity.HasOne(r => r.Business)
                    .WithMany(b => b.Reviews)
                    .HasForeignKey(r => r.BusinessId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.User)
                    .WithMany(u => u.Reviews)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Favorite>(entity =>
            {
                entity.HasIndex(f => new { f.UserId, f.BusinessId }).IsUnique();

                entity.HasOne(f => f.Business)
                    .WithMany()
                    .HasForeignKey(f => f.BusinessId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(f => f.User)
                    .WithMany(u => u.Favorites)
                    .HasForeignKey(f => f.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<BusinessStat>(entity =>
            {
                entity.HasIndex(s => s.BusinessId).IsUnique();
            });
        }
    }
}
