using PriceNegotiationAPI.Enums;

namespace PriceNegotiationAPI.Model
{
    public class Product
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public required string Name { get; set; }
        public string? Description { get; set; }
        public decimal InitialPrice { get; set; }
        public ProductStatus ProductStatus { get; set; }

        public List<Negotiation> Negotiations { get; set; } = new();
    }
}
