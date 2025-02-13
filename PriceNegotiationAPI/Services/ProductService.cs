using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PriceNegotiationAPI.Database;
using PriceNegotiationAPI.Enums;
using PriceNegotiationAPI.Model;
using PriceNegotiationAPI.Model.DTO;

namespace PriceNegotiationAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public ProductService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ProductDto> AddProductAsync(CreateProductDto createProductDto)
        {
            var product = new Product
            {
                Name = createProductDto.Name,
                Description = createProductDto.Description,
                InitialPrice = createProductDto.InitialPrice,
                ProductStatus = ProductStatus.Available
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<IEnumerable<ProductDto>> GetProductListAsync(int page, int pageSize)
        {
            var query = _context.Products.AsQueryable();

            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetProductAsync(string productId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
            return _mapper.Map<ProductDto>(product);
        }
    }
}
