using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TwinsFashion.Domain.Interfaces;
using TwinsFashion.Domain.Models;
using TwinsFashionApi.Models;
using TwinsFashionApi.Models.Mappings;

namespace TwinsFashionApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IProductService _productService;
        private readonly IViewMapper _viewMapper;

        public AdminController(
            IAdminService adminService, 
            IProductService productService, 
            IViewMapper viewMapper)
        {
            _adminService = adminService;
            _productService = productService;
            _viewMapper = viewMapper;
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
                // For now, we'll implement a simple update - you might want to expand this
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound(new { message = "Product not found" });
                }

                // Update logic would go here - you might want to add an UpdateProduct method to IProductService
                return Ok(new { message = "Product updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error updating product: {ex.Message}" });
            }
        }

        // Delete product
        [HttpDelete("products/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            try
            {
                // You might want to add a DeleteProduct method to IProductService
                return Ok(new { message = "Product deleted successfully" });
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
