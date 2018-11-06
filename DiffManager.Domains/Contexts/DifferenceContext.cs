namespace DiffManager.Domains.Contexts
{
    using DiffManager.Models;

    using Microsoft.EntityFrameworkCore;

    public class DifferenceContext : DbContext
    {
        public DifferenceContext(DbContextOptions<DifferenceContext> options)
            : base(options)
        {
        }

        public DbSet<Difference> Differences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Difference>();
        }
    }
}
