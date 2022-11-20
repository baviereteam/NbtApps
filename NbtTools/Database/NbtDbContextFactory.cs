using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtTools.Database
{
    /// <summary>
    /// Used to create the DbContext for design-time tools.
    /// </summary>
    public class NbtDbContextFactory : IDesignTimeDbContextFactory<NbtDbContext>
    {
        public NbtDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<NbtDbContext>();
            // Change this to your own path!
            optionsBuilder.UseSqlite("Filename=C:\\Users\\Cycy\\Source\\Repos\\NbtApps\\NbtTools\\Database\\nbt.db");

            return new NbtDbContext(optionsBuilder.Options);
        }
    }
}
