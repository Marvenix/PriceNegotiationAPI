using Microsoft.AspNetCore.Identity;

namespace PriceNegotiationAPI.Model
{
    public class ApplicationUser : IdentityUser
    {
        public List<Negotiation> Negotiations { get; set; } = new();
        public List<NegotiationOffer> NegotiationOffers { get; set; } = new();
    }
}
