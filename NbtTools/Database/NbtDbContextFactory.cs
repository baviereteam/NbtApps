using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NbtTools.Database
{
    /// <summary>
    /// Used to create the DbContext for design-time tools.
    /// </summary>
    public class NbtDbContextFactory : IDesignTimeDbContextFactory<NbtDbContext>
    {
        public NbtDbContext CreateDbContext(string[] args)
        {
            //TODO: see how to remove this! (how did I do it in McMerchantsLib?)
            var optionsBuilder = new DbContextOptionsBuilder<NbtDbContext>();
            // Change this to your own path!
            optionsBuilder.UseSqlite("Filename=C:\\Users\\Cycy\\Source\\Repos\\NbtApps\\NbtTools\\Database\\nbt.db");

            return new NbtDbContext(optionsBuilder.Options);
        }
    }
}
