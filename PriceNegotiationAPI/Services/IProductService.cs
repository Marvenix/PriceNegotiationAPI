using PriceNegotiationAPI.Model.DTO;

namespace PriceNegotiationAPI.Services
{
    public interface IProductService
    {
        Task<ProductDto> AddProductAsync(CreateProductDto product);
        Task<ProductDto?> GetProductAsync(string productId);
        Task<IEnumerable<ProductDto>> GetProductListAsync(int page, int pageSize);

    }
}
