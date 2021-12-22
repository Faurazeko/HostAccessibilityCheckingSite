using HostAccessibilityCheckingSite.Data.Models;

using Microsoft.EntityFrameworkCore;

using System;

namespace HostAccessibilityCheckingSite
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<PingResult> PingHistory { get; set; }
        public DbSet<SiteSettings> Sites { get; set; }
        public DbSet<Relation> Relations { get; set; }

        public AppDbContext() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=HostChekingDb;Trusted_Connection=True;MultipleActiveResultSets=true");
        }
    }
}