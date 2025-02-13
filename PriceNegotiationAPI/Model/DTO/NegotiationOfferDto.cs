namespace PriceNegotiationAPI.Model.DTO
{
    public class NegotiationOfferDto
    {
        public string? Id { get; set; }
        public required string NegotiationId { get; set; }
        public required Negotiation Negotiation { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
