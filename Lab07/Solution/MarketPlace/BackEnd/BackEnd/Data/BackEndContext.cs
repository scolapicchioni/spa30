using Microsoft.EntityFrameworkCore;

namespace BackEnd.Models {
    public class BackEndContext : DbContext
    {
        public BackEndContext (DbContextOptions<BackEndContext> options)
            : base(options)
        {
        }

        public DbSet<BackEnd.Models.Product> Product { get; set; }
    }
}
