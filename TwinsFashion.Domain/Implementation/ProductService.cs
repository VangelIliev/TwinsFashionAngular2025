
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TwinsFashion.Data.Models;
using TwinsFashion.Domain.Interfaces;
using TwinsFashion.Domain.Mappers;
using TwinsFashion.Domain.Models;

namespace TwinsFashion.Domain.Implementation
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly IDomainMapper _mapper;
        private readonly ILogger<ProductService> _logger;

        public ProductService(AppDbContext context, IDomainMapper mapper, ILogger<ProductService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            try
            {
                var dataProducts = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Color)
                    .Include(p => p.Images)
                    .Include(p => p.Sizes)
                    .Include(p => p.SubCategory)
                    .ToListAsync();
                if (!dataProducts.Any())
                {
                    _logger.LogError("No products found in the database.");
                    return Enumerable.Empty<ProductDto>();
                }
                return _mapper.MapDomainProducts(dataProducts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving products.");
                throw;
            }
        }

        public async Task<ProductDto?> GetProductByIdAsync(Guid id)
        {
            try
            {
                var dataProduct = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Color)
                    .Include(p => p.Images)
                    .Include(p => p.Sizes)
                    .FirstOrDefaultAsync(p => p.Id == id);
                if (dataProduct == null)
                {
                    _logger.LogWarning("Product with ID {Id} not found.", id);
                    return null;
                }
                return _mapper.MapDomainProductDto(dataProduct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error occurred while retrieving product with ID {Id}.", id);
                throw;
            }
        }

        public async Task<IEnumerable<SubCategoryDto>> GetSubCategories()
        {
            try
            {
                var subCategories = await _context.SubCategories
                    .Include(sc => sc.Category)
                    .ToListAsync();
                if (!subCategories.Any())
                {
                    _logger.LogWarning("No subcategories found in the database.");
                    return Enumerable.Empty<SubCategoryDto>();
                }
                return _mapper.MapDomainSubCategories(subCategories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving subcategories.");
                throw;
            }
        }

        public async Task<IEnumerable<ColorDto>> GetColors()
        {
            try
            {
                var colors = await _context.Colors.ToListAsync();
                if (!colors.Any())
                {
                    _logger.LogWarning("No colors found in the database.");
                    return Enumerable.Empty<ColorDto>();
                }
                return _mapper.MapDomainColors(colors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving colors.");
                throw;
            }
        }

        public async Task<IEnumerable<CategoryDto>> GetCategories()
        {
            try
            {
                var categories = await _context.Categories.ToListAsync();
                if (!categories.Any())
                {
                    _logger.LogWarning("No categories found in the database.");
                    return Enumerable.Empty<CategoryDto>();
                }
                return _mapper.MapDomainCategories(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving categories.");
                throw;
            }
        }

        public async Task<IEnumerable<SizeDto>> GetSizes()
        {
            try
            {
                var sizes = await _context.Sizes.ToListAsync();
                if (!sizes.Any())
                {
                    _logger.LogWarning("No sizes found in the database.");
                    return Enumerable.Empty<SizeDto>();
                }
                return _mapper.MapDomainSizes(sizes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving sizes.");
                throw;
            }
        }

        public async Task<bool> SeedProductToDatabase(string categoryName, string colorName, string subcategoryName, IEnumerable<SizeDto> sizes)
        {
            try
            {
                var category = await _context.Categories.FirstAsync(x => x.Name == categoryName);
                var color = await _context.Colors.FirstAsync(x => x.Name == colorName);
                var subCategory = await _context.SubCategories.FirstAsync(x => x.Name == subcategoryName);

                var productId = Guid.NewGuid();

                var images = new List<Image>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productId,
                        Url = "/images/pants/Elizabeth_Franchie_Pants.jpg"
                    }
                };

                var sizeEntities = (sizes ?? Enumerable.Empty<SizeDto>())
                    .Select(dto => new Size
                    {
                        Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                        Name = JsonConvert.SerializeObject(new Dictionary<string, string>
                        {
                            { "type", dto.Type },
                            { "size", dto.Size }
                        })
                    })
                    .ToList();

                var product = new Product
                {
                    Id = productId,
                    Name = "Панталон Елизабетa Франчи",
                    Description = "Летен панталон от памук и еластан",
                    Price = 110,
                    Quantity = 10,
                    CategoryId = category.Id,
                    Category = category,
                    ColorId = color.Id,
                    Color = color,
                    SubCategoryId = subCategory.Id,
                    SubCategory = subCategory,
                    Images = images,
                    Sizes = sizeEntities
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while seeding products.");
                throw;
            }
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(string categoryName)
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Color)
                    .Include(p => p.Images)
                    .Include(p => p.Sizes)
                    .Where(p => p.Category.Name == categoryName)
                    .ToListAsync();
                if (!products.Any())
                {
                    _logger.LogWarning("No products found for category {CategoryName}.", categoryName);
                    return Enumerable.Empty<ProductDto>();
                }
                return _mapper.MapDomainProducts(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving products by category {CategoryName}.", categoryName);
                throw;
            }
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByColorAsync(string colorName)
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Color)
                    .Include(p => p.Images)
                    .Include(p => p.Sizes)
                    .Where(p => p.Color.Name == colorName)
                    .ToListAsync();
                if (!products.Any())
                {
                    _logger.LogWarning("No products found for color {ColorName}.", colorName);
                    return Enumerable.Empty<ProductDto>();
                }
                return _mapper.MapDomainProducts(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving products by color {ColorName}.", colorName);
                throw;
            }
        }

        public async Task<bool> AddProductInDatabase(
            string name,
            string description,
            int price,
            int quantity,
            Guid categoryId,
            Guid subCategoryId,
            Guid colorId,
            IEnumerable<string> imageUrls,
            IEnumerable<Guid> sizeIds)
        {
            try
            {
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
                var color = await _context.Colors.FirstOrDefaultAsync(c => c.Id == colorId);
                var subCategory = await _context.SubCategories.FirstOrDefaultAsync(sc => sc.Id == subCategoryId);
                if (category == null || color == null || subCategory == null)
                {
                    _logger.LogWarning(
                        "Invalid related entity id(s). Category: {CategoryId} exists: {HasCategory}, Color: {ColorId} exists: {HasColor}, SubCategory: {SubCategoryId} exists: {HasSubCategory}",
                        categoryId, category != null, colorId, color != null, subCategoryId, subCategory != null);
                    return false;
                }

                var sizeIdList = (sizeIds ?? Enumerable.Empty<Guid>()).Distinct().ToList();
                List<Size> sizes;
                if (sizeIdList.Count == 0)
                {
                    sizes = new List<Size>();
                }
                else
                {
                    var param = Expression.Parameter(typeof(Size), "s");
                    Expression body = Expression.Constant(false);
                    foreach (var id in sizeIdList)
                    {
                        var left = Expression.Property(param, nameof(Size.Id));
                        var right = Expression.Constant(id);
                        var eq = Expression.Equal(left, right);
                        body = Expression.OrElse(body, eq);
                    }
                    var lambda = Expression.Lambda<Func<Size, bool>>(body, param);
                    sizes = await _context.Sizes.Where(lambda).ToListAsync();
                }

                if (sizes.Count != sizeIdList.Count)
                {
                    var missing = sizeIdList.Except(sizes.Select(s => s.Id)).ToArray();
                    _logger.LogWarning("Some size ids were not found: {Missing}", string.Join(",", missing));
                }

                var productId = Guid.NewGuid();
                var normalizedUrls = (imageUrls ?? Enumerable.Empty<string>())
                    .Where(u => !string.IsNullOrWhiteSpace(u))
                    .Select(u =>
                    {
                        var url = u.Replace("\\", "/").Trim();
                        var idx = url.IndexOf("wwwroot/", StringComparison.OrdinalIgnoreCase);
                        if (idx >= 0)
                        {
                            url = url[(idx + "wwwroot".Length)..];
                        }
                        if (!url.StartsWith('/'))
                        {
                            url = "/" + url.TrimStart('~', '/');
                        }
                        return url;
                    })
                    .Distinct()
                    .ToList();

                var images = normalizedUrls.Select(url => new Image
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    Url = url
                }).ToList();

                var product = new Product
                {
                    Id = productId,
                    Name = name,
                    Description = description,
                    Price = price,
                    Quantity = quantity,
                    CategoryId = category.Id,
                    Category = category,
                    ColorId = color.Id,
                    Color = color,
                    SubCategoryId = subCategory.Id,
                    SubCategory = subCategory,
                    Images = images,
                    Sizes = sizes
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding product to database");
                return false;
            }
        }

        public async Task<bool> CreateCategoryAsync(string name)
        {
            try
            {
                name = (name ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    return false;
                }
                var exists = await _context.Categories.AnyAsync(c => c.Name == name);
                if (exists)
                {
                    return true;
                }
                _context.Categories.Add(new Category { Id = Guid.NewGuid(), Name = name });
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category {Name}", name);
                return false;
            }
        }

        public async Task<bool> CreateColorAsync(string name)
        {
            try
            {
                name = (name ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    return false;
                }
                var exists = await _context.Colors.AnyAsync(c => c.Name == name);
                if (exists)
                {
                    return true;
                }
                _context.Colors.Add(new Color { Id = Guid.NewGuid(), Name = name });
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating color {Name}", name);
                return false;
            }
        }

        public async Task<bool> CreateSizeAsync(string type, string size)
        {
            try
            {
                type = (type ?? string.Empty).Trim();
                size = (size ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(size))
                {
                    return false;
                }
                var nameJson = JsonConvert.SerializeObject(new Dictionary<string, string>
                {
                    { "type", type },
                    { "size", size }
                });
                var exists = await _context.Sizes.AnyAsync(s => s.Name == nameJson);
                if (exists)
                {
                    return true;
                }
                _context.Sizes.Add(new Size { Id = Guid.NewGuid(), Name = nameJson });
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating size {Type}-{Size}", type, size);
                return false;
            }
        }

        public async Task<bool> CreateSubCategoryAsync(Guid categoryId, string name)
        {
            try
            {
                name = (name ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(name) || categoryId == Guid.Empty)
                {
                    return false;
                }
                var catExists = await _context.Categories.AnyAsync(c => c.Id == categoryId);
                if (!catExists)
                {
                    return false;
                }
                var exists = await _context.SubCategories.AnyAsync(sc => sc.Name == name && sc.CategoryId == categoryId);
                if (exists)
                {
                    return true;
                }
                _context.SubCategories.Add(new SubCategory
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    CategoryId = categoryId
                });
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subcategory {Name} for category {CategoryId}", name, categoryId);
                return false;
            }
        }
    }
}
