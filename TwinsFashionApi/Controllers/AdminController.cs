using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Net;
using System.Security.Claims;
using TwinsFashion.Domain.Interfaces;
using TwinsFashion.Domain.Models;
using TwinsFashionApi.Models;
using TwinsFashionApi.Models.Mappings;
using TwinsFashionApi.Options;

namespace TwinsFashionApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IProductService _productService;
        private readonly IViewMapper _viewMapper;
        private readonly IMemoryCache _cache;
        private const string AllProductsCacheKey = "products:all";
        private const string ProductByIdCachePrefix = "products:id:";
        private readonly Cloudinary _cloudinary;
        private readonly CloudinaryOptions _cloudinaryOptions;

        public AdminController(
            IAdminService adminService, 
            IProductService productService, 
            IViewMapper viewMapper,
            Cloudinary cloudinary,
            IOptions<CloudinaryOptions> cloudinaryOptions,
            IMemoryCache cache)
        {
            _adminService = adminService;
            _productService = productService;
            _viewMapper = viewMapper;
            _cache = cache;
            _cloudinary = cloudinary;
            _cloudinaryOptions = cloudinaryOptions.Value;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LogIn([FromBody] AdminLoginRequest request)
        {
            Console.WriteLine($"Login attempt for username: {request.Username}");
            
            if (await _adminService.AuthoriseUser(request.Username, request.Password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, request.Username),
                    new Claim(ClaimTypes.Role, "Admin")
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMonths(1),
                    AllowRefresh = true,
                    IssuedUtc = DateTimeOffset.UtcNow
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                Console.WriteLine($"Login successful for user: {request.Username}");
                return Ok(new { message = "Login successful", username = request.Username });
            }
            Console.WriteLine($"Login failed for user: {request.Username}");
            return BadRequest(new { message = "Невалиден username или парола" });
        }

        [HttpPost("logout")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { message = "Logout successful" });
        }

        [HttpGet("check-auth")]
        public IActionResult CheckAuth()
        {
            var isAuthenticated = User.Identity?.IsAuthenticated == true;
            var isAdmin = User.IsInRole("Admin");
            var username = User.Identity?.Name;
            
            // Debug information
            Console.WriteLine($"CheckAuth - IsAuthenticated: {isAuthenticated}, IsAdmin: {isAdmin}, Username: {username}");
            Console.WriteLine($"Request Headers - Cookie: {Request.Headers["Cookie"]}");
            Console.WriteLine($"User Identity Name: {User.Identity?.Name}");
            Console.WriteLine($"User Identity IsAuthenticated: {User.Identity?.IsAuthenticated}");
            Console.WriteLine($"User Claims: {string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}"))}");
            
            if (isAuthenticated && isAdmin)
            {
                return Ok(new { isAuthenticated = true, username = username });
            }
            return Ok(new { isAuthenticated = false });
        }

        // Dashboard - Get all products
        [HttpGet("dashboard/products")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDashboardProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            
            // Debug information
            foreach (var product in products)
            {
                Console.WriteLine($"Product: {product.Name}, Category: {product.Category?.Name ?? "NULL"}, Category Object: {product.Category}");
            }
            
            var viewModels = _viewMapper.MapAdminViewModelProducts(products);
            return Ok(viewModels);
        }

        // Get all categories
        [HttpGet("categories")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _productService.GetCategories();
            var viewModels = _viewMapper.MapViewModelCategories(categories);
            return Ok(viewModels);
        }

        // Get all subcategories
        [HttpGet("subcategories")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetSubCategories()
        {
            var subcategories = await _productService.GetSubCategories();
            var viewModels = _viewMapper.MapViewModelSubCategories(subcategories);
            return Ok(viewModels);
        }

        // Get all colors
        [HttpGet("colors")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetColors()
        {
            var colors = await _productService.GetColors();
            var viewModels = _viewMapper.MapViewModelColors(colors);
            return Ok(viewModels);
        }

        // Get all sizes
        [HttpGet("sizes")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetSizes()
        {
            var sizes = await _productService.GetSizes();
            return Ok(sizes);
        }

        // Upload images to Cloudinary
        [HttpPost("upload-images")]
        [Authorize(Roles = "Admin")]
        [RequestSizeLimit(20_000_000)]
        public async Task<IActionResult> UploadImages([FromForm] List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest(new { message = "Не са предоставени файлове" });
            }

            var uploadResults = new List<object>();

            foreach (var file in files)
            {
                if (file.Length == 0)
                {
                    continue;
                }

                await using var stream = file.OpenReadStream();

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = string.IsNullOrWhiteSpace(_cloudinaryOptions.Folder)
                        ? null
                        : _cloudinaryOptions.Folder
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode != HttpStatusCode.OK)
                {
                    return BadRequest(new
                    {
                        message = $"Грешка при качване на {file.FileName}",
                        error = uploadResult.Error?.Message
                    });
                }

                uploadResults.Add(new
                {
                    url = uploadResult.SecureUrl?.ToString() ?? uploadResult.Url?.ToString() ?? string.Empty,
                    publicId = uploadResult.PublicId
                });
            }

            if (!uploadResults.Any())
            {
                return BadRequest(new { message = "Неуспешно качване на снимките" });
            }

            return Ok(uploadResults);
        }

        // Add new product
        [HttpPost("products")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddProduct([FromBody] AdminAddProductRequest request)
        {
            try
            {
                var success = await _productService.AddProductInDatabase(
                    request.Name,
                    request.Description,
                    request.Price,
                    request.Quantity,
                    request.CategoryId,
                    request.SubCategoryId,
                    request.ColorId,
                    request.ImageUrls,
                    request.SizeIds
                );

                if (success)
                {
                    _cache.Remove(AllProductsCacheKey);
                    return Ok(new { message = "Product added successfully" });
                }
                return BadRequest(new { message = "Failed to add product" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error adding product: {ex.Message}" });
            }
        }

        // Update product
        [HttpPut("products/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] AdminUpdateProductRequest request)
        {
            try
            {
                var success = await _productService.UpdateProductAsync(
                    id,
                    string.IsNullOrWhiteSpace(request.Name) ? null : request.Name,
                    request.Price,
                    request.SubCategoryId,
                    request.SizeIds);

                if (success)
                {
                    _cache.Remove(AllProductsCacheKey);
                    _cache.Remove(ProductByIdCachePrefix + id);
                    return Ok(new { message = "Product updated successfully" });
                }

                return BadRequest(new { message = "Failed to update product" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error updating product: {ex.Message}" });
            }
        }

        // Delete product
        [HttpDelete("products/{id}")]
        [Authorize(Roles = "Admin")]
        public Task<IActionResult> DeleteProduct(Guid id)
        {
            return HandleDeleteProduct(id);
        }

        // Fallback delete endpoint for hosts that do not allow DELETE
        [HttpPost("products/{id}/delete")]
        [Authorize(Roles = "Admin")]
        public Task<IActionResult> DeleteProductViaPost(Guid id)
        {
            return HandleDeleteProduct(id);
        }

        private async Task<IActionResult> HandleDeleteProduct(Guid id)
        {
            try
            {
                var deleted = await _productService.DeleteProductAsync(id);
                if (deleted)
                {
                    _cache.Remove(AllProductsCacheKey);
                    _cache.Remove(ProductByIdCachePrefix + id);
                    return Ok(new { message = "Product deleted successfully" });
                }

                return NotFound(new { message = "Product not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error deleting product: {ex.Message}" });
            }
        }

        // Add category
        [HttpPost("categories")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddCategory([FromBody] CreateCategoryRequest request)
        {
            try
            {
                var success = await _productService.CreateCategoryAsync(request.Name);
                if (success)
                {
                    return Ok(new { message = "Category added successfully" });
                }
                return BadRequest(new { message = "Failed to add category" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error adding category: {ex.Message}" });
            }
        }

        // Add subcategory
        [HttpPost("subcategories")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddSubCategory([FromBody] CreateSubCategoryRequest request)
        {
            try
            {
                var success = await _productService.CreateSubCategoryAsync(request.CategoryId, request.Name);
                if (success)
                {
                    return Ok(new { message = "Subcategory added successfully" });
                }
                return BadRequest(new { message = "Failed to add subcategory" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error adding subcategory: {ex.Message}" });
            }
        }

        // Add color
        [HttpPost("colors")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddColor([FromBody] CreateColorRequest request)
        {
            try
            {
                var success = await _productService.CreateColorAsync(request.Name);
                if (success)
                {
                    return Ok(new { message = "Color added successfully" });
                }
                return BadRequest(new { message = "Failed to add color" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error adding color: {ex.Message}" });
            }
        }

        // Add size
        [HttpPost("sizes")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddSize([FromBody] CreateSizeRequest request)
        {
            try
            {
                var success = await _productService.CreateSizeAsync(request.Type, request.Size);
                if (success)
                {
                    return Ok(new { message = "Size added successfully" });
                }
                return BadRequest(new { message = "Failed to add size" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error adding size: {ex.Message}" });
            }
        }

        // Set cover image for product
        [HttpPost("products/{productId}/set-cover-image")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetCoverImage(Guid productId, [FromBody] SetCoverImageRequest request)
        {
            try
            {
                var success = await _productService.SetCoverImageAsync(productId, request.ImageId);
                if (success)
                {
                    _cache.Remove(AllProductsCacheKey);
                    _cache.Remove(ProductByIdCachePrefix + productId);
                    return Ok(new { message = "Cover image set successfully" });
                }
                return BadRequest(new { message = "Failed to set cover image. Product or image not found." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error setting cover image: {ex.Message}" });
            }
        }
    }
}
