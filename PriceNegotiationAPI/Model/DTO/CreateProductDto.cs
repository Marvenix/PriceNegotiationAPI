namespace PriceNegotiationAPI.Model.DTO
{
    public class CreateProductDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public decimal InitialPrice { get; set; }
    }
}
