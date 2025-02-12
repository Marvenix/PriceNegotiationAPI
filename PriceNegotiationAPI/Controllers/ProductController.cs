using Microsoft.AspNetCore.Mvc;
using PriceNegotiationAPI.Model;
using PriceNegotiationAPI.Model.DTO;
using PriceNegotiationAPI.Services;

namespace PriceNegotiationAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost("/addproduct")]
        public async Task<ActionResult<ProductDto>> AddProduct([FromBody] CreateProductDto createProductDto)
        {
            if (createProductDto == null)
                return BadRequest("Couldn't get data from body.");

            if (String.IsNullOrEmpty(createProductDto.Name))
                return BadRequest("Incomplete data.");

            var productDto = await _productService.AddProductAsync(createProductDto);

            return CreatedAtAction("GetProduct", new { productId = productDto.Id }, productDto);
        }

        [HttpGet("/getproductlist")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductList([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var productList = await _productService.GetProductListAsync(page, pageSize);

            return Ok(productList);
        }

        [HttpGet("/getproduct/{productId}")]
        public async Task<ActionResult<ProductDto>> GetProduct(string productId)
        {
            var product = await _productService.GetProductAsync(productId);

            if (product == null)
                return NotFound();

            return Ok(product);
        }
    }
}
