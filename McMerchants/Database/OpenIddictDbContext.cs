using Microsoft.EntityFrameworkCore;

namespace McMerchants.Database
{
    public class OpenIddictDbContext : DbContext
    {
        public OpenIddictDbContext(DbContextOptions<OpenIddictDbContext> options) : base(options) { }
    }
}
