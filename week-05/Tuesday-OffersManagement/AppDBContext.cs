using Microsoft.EntityFrameworkCore;

namespace OffersManagement
{
    public class AppDbContext : DbContext
    {
        public DbSet<Offer> Offers { get; set; }

        public DbSet<PlanOffer> PlanOffers { get; set; }

        public DbSet<OfferPlanOffer> OfferPlanOffers { get; set; }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=WIN764OMNIA;Database=OffersDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(
    ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OfferPlanOffer>()
                .HasKey(x => new { x.OfferId, x.PlanOfferId });

            modelBuilder.Entity<OfferPlanOffer>()
                .HasOne(x => x.Offer)
                .WithMany(o => o.OfferPlanOffers)
                .HasForeignKey(x => x.OfferId);

            modelBuilder.Entity<OfferPlanOffer>()
                .HasOne(x => x.PlanOffer)
                .WithMany(p => p.OfferPlanOffers)
                .HasForeignKey(x => x.PlanOfferId);
        }
    }
}