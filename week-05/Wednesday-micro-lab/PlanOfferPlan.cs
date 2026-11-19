namespace OffersManagement
{
    public class OfferPlanOffer
    {
        public int OfferId { get; set; }

        public int PlanOfferId { get; set; }

        // Navigation properties
        public Offer Offer { get; set; }

        public PlanOffer PlanOffer { get; set; }
    }
}