using Microsoft.EntityFrameworkCore;
using NbtTools.Items;

namespace NbtTools.Database
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
    public class NbtDbContext : DbContext
    {
        public DbSet<Searchable> Searchables { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Potion> Potions { get; set; }
        public DbSet<EnchantedBook> EnchantedBooks { get; set; }

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
            modelBuilder.Entity<Potion>(entity => {
                entity.ToTable("potions");
                entity.HasOne(potion => potion.Type);
            });
            modelBuilder.Entity<EnchantedBook>().ToTable("enchanted_books");

            base.OnModelCreating(modelBuilder);
        }
    }
}
