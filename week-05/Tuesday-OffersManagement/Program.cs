
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace OffersManagement
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new AppDbContext())
            {
                // Create Offers
                var offer1 = new Offer
                {
                    OfferName = "Summer Discount",
                    Discount = 20
                };

                var offer2 = new Offer
                {
                    OfferName = "New Customer Offer",
                    Discount = 10
                };

                // Create Plans
                var plan1 = new PlanOffer
                {
                    PlanName = "Premium Plan"
                };

                var plan2 = new PlanOffer
                {
                    PlanName = "Basic Plan"
                };

                // Connect Offers and Plans
                var relation1 = new OfferPlanOffer
                {
                    Offer = offer1,
                    PlanOffer = plan1
                };

                var relation2 = new OfferPlanOffer
                {
                    Offer = offer2,
                    PlanOffer = plan1
                };

                var relation3 = new OfferPlanOffer
                {
                    Offer = offer1,
                    PlanOffer = plan2
                };

                // Save all records
                context.Offers.AddRange(offer1, offer2);

                context.PlanOffers.AddRange(plan1, plan2);

                context.OfferPlanOffers.AddRange(
                    relation1,
                    relation2,
                    relation3
                );

                context.SaveChanges();

                Console.WriteLine(
                    "Records inserted successfully!"
                );

                // Retrieve data
                var results = context.PlanOffers
                    .Include(p => p.OfferPlanOffers)
                    .ThenInclude(po => po.Offer)
                    .ToList();

                // Display Plans and their Offers
                foreach (var plan in results)
                {
                    Console.WriteLine(
                        "\nPlan: " + plan.PlanName
                    );

                    foreach (var relation in plan.OfferPlanOffers)
                    {
                        Console.WriteLine(
                            relation.Offer.OfferName + " - " +
                            relation.Offer.Discount + "%"
                        );
                    }
                }

                Console.ReadLine();
            }
        }
    }
}
