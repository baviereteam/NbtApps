using Microsoft.EntityFrameworkCore;
using NbtTools.Items;

namespace NbtTools.Database
{
    /**
     * In order to generate migrations, a terminal must be opened in the McMerchants project.
     * Use the --project switch to target the NbtTools as migration target:
     * PS C:\NbtApps\McMerchants> dotnet ef migrations add InitialCreate --project ../NbtTools
     **/

    public class NbtDbContext : DbContext
    {
        public DbSet<Item> Items { get; set; }

        public NbtDbContext(DbContextOptions<NbtDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Searchable>().UseTpcMappingStrategy();
            modelBuilder.Entity<Searchable>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name);
            });
            modelBuilder.Entity<Item>().ToTable("items");

            base.OnModelCreating(modelBuilder);
        }
    }
}
