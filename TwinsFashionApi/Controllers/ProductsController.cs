using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using TwinsFashion.Domain.Interfaces;
using TwinsFashion.Domain.Models;

namespace TwinsFashionApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMemoryCache _cache;

        private const string AllProductsCacheKey = "products:all";
        private const string ProductSummaryCacheKey = "products:summary";
        private const string ProductByIdCachePrefix = "products:id:";

        public ProductsController(IProductService productService, IMemoryCache cache)
        {
            _productService = productService;
            _cache = cache;
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            if (_cache.TryGetValue(AllProductsCacheKey, out IEnumerable<ProductDto>? cached) && cached != null)
            {
                return Ok(cached);
            }

            var products = await _productService.GetAllProductsAsync();

            if (products == null)
            {
                products = Enumerable.Empty<ProductDto>();
            }

            _cache.Set(AllProductsCacheKey, products.ToList(), new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
            });

            return Ok(products);
        }

        [HttpGet("summary")]
        public async Task<ActionResult<IEnumerable<ProductSummaryDto>>> GetSummary()
        {
            if (_cache.TryGetValue(ProductSummaryCacheKey, out IEnumerable<ProductSummaryDto>? cached) && cached != null)
            {
                return Ok(cached);
            }

            var products = await _productService.GetProductSummariesAsync();
            if (products == null)
            {
                products = Enumerable.Empty<ProductSummaryDto>();
            }

            _cache.Set(ProductSummaryCacheKey, products.ToList(), new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
            });

            return Ok(products);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProductDto>> GetById(Guid id)
        {
            var cacheKey = ProductByIdCachePrefix + id;
            if (_cache.TryGetValue(cacheKey, out ProductDto? cached) && cached != null)
            {
                return Ok(cached);
            }

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _cache.Set(cacheKey, product, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(120)
            });

            return Ok(product);
        }
    }
}
