using PriceNegotiationAPI.Enums;

namespace PriceNegotiationAPI.Model.DTO
{
    public class NegotiationDto
    {
        public string? Id { get; set; } 
        public required string UserId { get; set; }
        public required ApplicationUser CreatedBy { get; set; }
        public required string ProductId { get; set; }
        public required Product Product { get; set; }
        public NegotiationStatus NegotiationStatus { get; set; }
        public int RejectsCounter { get; set; }
        public DateTime ValidUntil { get; set; }
    }
}
