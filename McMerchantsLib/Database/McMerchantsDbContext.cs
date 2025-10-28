using McMerchants.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace McMerchants.Database
{
    /**
      When executing Entity Framework Core commands, you must set the `project`, `startup-project` and `context` parameters so that:
        * `project` points to the project in which files (migrations, ...) must be read and written,
        * `context` is the name of the DbContext describing the database to operate on,
        * `startup-project` is the name of the project containing a reference to `Microsoft.EntityFrameworkCore.Design` package.

        For example:
        ```
        dotnet ef database update --project NbtTools --startup-project McMerchantsLib --context NbtDbContext
        ```

        If the `startup-project` is missing, you will get the following error:
        > Your startup project 'NbtTools' doesn't reference Microsoft.EntityFrameworkCore.Design. This package is required for the Entity Framework Core Tools to work. Ensure your startup project is correct, install the package, and try again.
    **/

    /**
     * To run the migrations for a database in a given environment:
     * PM> $env:DOTNET_ENVIRONMENT="Test"
     * PM> dotnet ef database update ...
    **/

    public class McMerchantsDbContext : DbContext
    {
        public DbSet<StorageRegion> StorageRegions { get; set; }
        public DbSet<Alley> Alleys { get; set; }
        public DbSet<StoreItemDefaultAlley> DefaultAlleys { get; set; }

        public DbSet<TradingRegion> TradingRegions { get; set; }

        public DbSet<FactoryRegion> FactoryRegions { get; set; }

        public DbSet<FactoryProduct> FactoryProducts { get; set; }

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
                .HasValue<TradingRegion>("trading")
                .HasValue<FactoryRegion>("factory");

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

                entity.HasMany(s => s.Alleys)
                    .WithOne(a => a.Store)
                    .HasForeignKey(a => a.StoreId)
                    .IsRequired();
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

            modelBuilder.Entity<FactoryRegion>(entity =>
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

            modelBuilder.Entity<FactoryProduct>(entity =>
            {
                entity
                    .ToTable("factory_products")
                    .HasKey(e => e.Id);
                entity
                    .HasOne(p => p.Factory)
                    .WithMany(f => f.Products);
            });

            modelBuilder.Entity<Alley>(entity =>
            {
                entity
                    .ToTable("alleys")
                    .HasKey(e => e.Id);
                entity.HasOne(a => a.Store)
                    .WithMany(s => s.Alleys);
            });

            modelBuilder.Entity<StoreItemDefaultAlley>(entity => 
            {
                entity
                    .ToTable("default_alley")
                    .HasKey(d => d.Id);

                entity.HasOne(d => d.Alley);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
