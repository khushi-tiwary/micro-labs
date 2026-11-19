using System.Collections.Generic;

namespace OffersManagement
{
    public class PlanOffer
    {
        public int PlanOfferId { get; set; }

        public string PlanName { get; set; }

        // Navigation property
        public ICollection<OfferPlanOffer> OfferPlanOffers { get; set; }
            = new List<OfferPlanOffer>();
    }
}