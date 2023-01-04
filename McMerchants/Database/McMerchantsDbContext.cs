using McMerchants.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace McMerchants.Database
{
    public class McMerchantsDbContext : DbContext
    {
        public DbSet<StorageRegion> StorageRegions { get; set; }
        public DbSet<TradingRegion> TradingRegions { get; set; }

        public McMerchantsDbContext(DbContextOptions<McMerchantsDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ItemProviderRegion>()
                .Property(i => i.Type).HasColumnName("type");

            modelBuilder.Entity<ItemProviderRegion>()
                .ToTable("item_provider_regions")
                .Ignore(region => region.Coordinates)
                .HasDiscriminator(i => i.Type)
                .HasValue<StorageRegion>("storage")
                .HasValue<TradingRegion>("trading");

            modelBuilder.Entity<ItemProviderRegion>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<StorageRegion>(entity =>
            {
                entity.HasBaseType(typeof(ItemProviderRegion));
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Logo).HasColumnName("logo");
                entity.Property(e => e.Dimension).HasColumnName("dimension");
                entity.Property(e => e.StartX).HasColumnName("startX");
                entity.Property(e => e.StartY).HasColumnName("startY");
                entity.Property(e => e.StartZ).HasColumnName("startZ");
                entity.Property(e => e.EndX).HasColumnName("endX");
                entity.Property(e => e.EndY).HasColumnName("endY");
                entity.Property(e => e.EndZ).HasColumnName("endZ");
            });

            modelBuilder.Entity<TradingRegion>(entity =>
            {
                entity.HasBaseType(typeof(ItemProviderRegion));
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Logo).HasColumnName("logo");
                entity.Property(e => e.Dimension).HasColumnName("dimension");
                entity.Property(e => e.StartX).HasColumnName("startX");
                entity.Property(e => e.StartY).HasColumnName("startY");
                entity.Property(e => e.StartZ).HasColumnName("startZ");
                entity.Property(e => e.EndX).HasColumnName("endX");
                entity.Property(e => e.EndY).HasColumnName("endY");
                entity.Property(e => e.EndZ).HasColumnName("endZ");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
