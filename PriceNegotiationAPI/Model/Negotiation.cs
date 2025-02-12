using PriceNegotiationAPI.Enums;

namespace PriceNegotiationAPI.Model
{
    public class Negotiation
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public required string UserId { get; set; }
        public required ApplicationUser CreatedBy { get; set; }
        public required string ProductId { get; set; }
        public required Product Product { get; set; }
        public NegotiationStatus NegotiationStatus { get; set; }
        public int RejectsCounter { get; set; }
        public DateTime ValidUntil {  get; set; }

        public List<NegotiationOffer> NegotiationOffers { get; set; } = new();
    }
}
