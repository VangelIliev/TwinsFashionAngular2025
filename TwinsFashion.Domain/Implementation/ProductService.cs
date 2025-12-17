using System;
using System.Linq;
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
                    .OrderBy(p => p.SubCategory.Name == "Кожени якета" ? 0 : 1) // Якета първи
                    .ThenBy(p => p.SubCategory.Name) // След това по азбучен ред на категориите
                    .ThenBy(p => p.Name) // И накрая по име на продукта
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
                        IsCover = false,
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
                name = (name ?? string.Empty).Trim();
                description = (description ?? string.Empty).Trim();

                // Defensive validation before hitting the database (prevents SQL truncation with clearer error messages)
                if (name.Length < 5 || name.Length > 150)
                {
                    throw new ArgumentException("Името трябва да е между 5 и 150 символа.", nameof(name));
                }

                if (description.Length < 10 || description.Length > 1000)
                {
                    throw new ArgumentException("Описанието трябва да е между 10 и 1000 символа.", nameof(description));
                }

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
                    .Select(u => u?.Replace("\\", "/").Trim())
                    .Where(u => !string.IsNullOrWhiteSpace(u))
                    .Select(url =>
                    {
                        if (Uri.TryCreate(url, UriKind.Absolute, out var absoluteUri) &&
                            (absoluteUri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase) ||
                             absoluteUri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase)))
                        {
                            return absoluteUri.ToString();
                        }

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
                    .Where(u => !string.IsNullOrWhiteSpace(u))
                    .Distinct()
                    .ToList();

                var tooLongUrl = normalizedUrls.FirstOrDefault(u => u.Length > 2048);
                if (tooLongUrl != null)
                {
                    throw new ArgumentException("Някоя от снимките има прекалено дълъг URL (над 2048 символа).", nameof(imageUrls));
                }

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
                _logger.LogError(ex,
                    "Error occurred while adding product to database. NameLength={NameLength}, DescLength={DescLength}, MaxUrlLength={MaxUrlLength}",
                    (name ?? string.Empty).Length,
                    (description ?? string.Empty).Length,
                    (imageUrls ?? Enumerable.Empty<string>()).Select(u => (u ?? string.Empty).Length).DefaultIfEmpty(0).Max());
                throw;
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

        public async Task<bool> SetCoverImageAsync(Guid productId, Guid imageId)
        {
            try
            {
                if (productId == Guid.Empty || imageId == Guid.Empty)
                {
                    return false;
                }

                // Check if product exists
                var productExists = await _context.Products.AnyAsync(p => p.Id == productId);
                if (!productExists)
                {
                    return false;
                }

                // Check if image exists and belongs to the product
                var image = await _context.Images
                    .FirstOrDefaultAsync(i => i.Id == imageId && i.ProductId == productId);
                if (image == null)
                {
                    return false;
                }

                // Set all images for this product to not be cover
                var allProductImages = await _context.Images
                    .Where(i => i.ProductId == productId)
                    .ToListAsync();

                foreach (var img in allProductImages)
                {
                    img.IsCover = false;
                }

                // Set the selected image as cover
                image.IsCover = true;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cover image {ImageId} for product {ProductId}", imageId, productId);
                return false;
            }
        }

        public async Task<bool> UpdateProductAsync(Guid productId, string? name, int? price, Guid? subCategoryId, IEnumerable<Guid>? sizeIds)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Sizes)
                    .FirstOrDefaultAsync(p => p.Id == productId);

                if (product == null)
                {
                    _logger.LogWarning("Attempted to update product {ProductId} but it was not found", productId);
                    return false;
                }

                var hasChanges = false;

                if (!string.IsNullOrWhiteSpace(name) && !string.Equals(product.Name, name, StringComparison.Ordinal))
                {
                    product.Name = name.Trim();
                    hasChanges = true;
                }

                if (price.HasValue && price.Value >= 0 && product.Price != price.Value)
                {
                    product.Price = price.Value;
                    hasChanges = true;
                }

                if (subCategoryId.HasValue && subCategoryId.Value != Guid.Empty && product.SubCategoryId != subCategoryId.Value)
                {
                    var subCategory = await _context.SubCategories.FirstOrDefaultAsync(sc => sc.Id == subCategoryId.Value);
                    if (subCategory == null)
                    {
                        _logger.LogWarning("Subcategory {SubCategoryId} not found when updating product {ProductId}", subCategoryId, productId);
                        return false;
                    }

                    product.SubCategoryId = subCategory.Id;
                    product.SubCategory = subCategory;
                    hasChanges = true;
                }

                if (sizeIds != null)
                {
                    var distinctIds = sizeIds.Where(id => id != Guid.Empty).Distinct().ToList();
                    var sizes = await _context.Sizes.Where(s => distinctIds.Contains(s.Id)).ToListAsync();

                    if (sizes.Count != distinctIds.Count)
                    {
                        var missing = distinctIds.Except(sizes.Select(s => s.Id)).ToArray();
                        _logger.LogWarning("Some size ids were not found when updating product {ProductId}: {Missing}", productId, string.Join(",", missing));
                    }

                    var currentIds = product.Sizes.Select(s => s.Id).ToHashSet();
                    var newIds = sizes.Select(s => s.Id).ToHashSet();

                    if (!currentIds.SetEquals(newIds))
                    {
                        product.Sizes.Clear();
                        foreach (var size in sizes)
                        {
                            product.Sizes.Add(size);
                        }
                        hasChanges = true;
                    }
                }

                if (!hasChanges)
                {
                    return true;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating product {ProductId}", productId);
                return false;
            }
        }

        public async Task<bool> DeleteProductAsync(Guid productId)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Images)
                    .Include(p => p.Sizes)
                    .FirstOrDefaultAsync(p => p.Id == productId);

                if (product == null)
                {
                    _logger.LogWarning("Attempted to delete product {ProductId} but it was not found", productId);
                    return false;
                }

                if (product.Sizes != null && product.Sizes.Any())
                {
                    // Clear many-to-many relationship entries
                    foreach (var size in product.Sizes.ToList())
                    {
                        product.Sizes.Remove(size);
                    }
                }

                if (product.Images != null && product.Images.Any())
                {
                    _context.Images.RemoveRange(product.Images);
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting product {ProductId}", productId);
                return false;
            }
        }
    }
}
