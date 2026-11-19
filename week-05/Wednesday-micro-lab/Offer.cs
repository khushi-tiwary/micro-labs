using System.Collections.Generic;

namespace OffersManagement
{
    public class Offer
    {
        public int OfferId { get; set; }

        public string OfferName { get; set; }

        public decimal Discount { get; set; }

        // Navigation property
        public ICollection<OfferPlanOffer> OfferPlanOffers { get; set; }
            = new List<OfferPlanOffer>();
    }
}