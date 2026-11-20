
using Microsoft.EntityFrameworkCore;
using Thursday_micro_lab.Entities;

namespace Thursday_micro_lab
{
    public class AppDbContext : DbContext
    {
        // Represents the EFCoreLabPlans SQL Server table.
        public DbSet<Plan> Plans { get; set; }

        // Configure SQL Server connection.
        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=WIN764OMNIA;
                  Database=TeleBill;
                  Trusted_Connection=True;
                  TrustServerCertificate=True;");
        }

        // Configure how the Plan entity maps to SQL Server.
        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            // Map the entity to our separate practice table.
            modelBuilder.Entity<Plan>()
                .ToTable("EFCoreLabPlans");

            // Configure the primary key.
            modelBuilder.Entity<Plan>()
                .HasKey(p => p.PlanId);

            // Configure RowVersion as a concurrency token.
            modelBuilder.Entity<Plan>()
                .Property(p => p.RowVersion)
                .IsRowVersion();
        }
    }
}
