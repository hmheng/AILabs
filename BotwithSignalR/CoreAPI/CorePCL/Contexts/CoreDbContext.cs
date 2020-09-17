using CorePCL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CorePCL.Contexts
{
    public class CoreDbContext : DbContext
    {
        public CoreDbContext(DbContextOptions<CoreDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Booking> Bookings { get; set; }
    }
}
