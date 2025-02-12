using PriceNegotiationAPI.Enums;

namespace PriceNegotiationAPI.Model.DTO
{
    public class ProductDto
    {
        public string? Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public decimal InitialPrice { get; set; }
        public ProductStatus ProductStatus { get; set; }
    }
}
