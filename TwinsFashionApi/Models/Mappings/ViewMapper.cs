using System;
using TwinsFashion.Domain.Models;

namespace TwinsFashionApi.Models.Mappings
{
    public class ViewMapper : IViewMapper
    {
        public IEnumerable<CategoryViewModel> MapViewModelCategories(IEnumerable<CategoryDto> categories)
        {
            return (categories ?? Enumerable.Empty<CategoryDto>())
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                });
        }

        public IEnumerable<ColorViewModel> MapViewModelColors(IEnumerable<ColorDto> colors)
        {
            return (colors ?? Enumerable.Empty<ColorDto>())
                .Select(c => new ColorViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                });
        }

        public ProductViewModel MapViewModelProduct(ProductDto? product)
        {
            if (product == null)
            {
                return new ProductViewModel();
            }

            return new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Category = product.Category?.Name ?? string.Empty,
                Subcategory = product.SubCategory?.Name ?? string.Empty,
                Color = product.Color?.Name ?? string.Empty,
                ImageUrls = product.Images.Select(i => i.Url).ToList(),
                CoverImageUrl = product.CoverImageUrl,
                Sizes = MapViewModelSizes(product.Sizes)
            };
        }

        public IEnumerable<ProductViewModel> MapViewModelProducts(IEnumerable<ProductDto> products)
        {
            return (products ?? Enumerable.Empty<ProductDto>())
                .Select(MapViewModelProduct)
                .ToList();
        }

        public List<string> MapViewModelSizes(IEnumerable<SizeDto> sizes)
        {
            return (sizes ?? Enumerable.Empty<SizeDto>())
                .Select(s => s.Size)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct()
                .ToList();
        }

        public IEnumerable<SubCategoryViewModel> MapViewModelSubCategories(IEnumerable<SubCategoryDto> subcategories)
        {
            return (subcategories ?? Enumerable.Empty<SubCategoryDto>())
                .Select(sc => new SubCategoryViewModel
                {
                    Id = sc.Id,
                    Name = sc.Name
                });
        }

        public AdminProductViewModel MapAdminViewModelProduct(ProductDto? product)
        {
            if (product == null)
            {
                return new AdminProductViewModel();
            }

            // Debug information
            Console.WriteLine($"Mapping product: {product.Name}, Category: {product.Category?.Name ?? "NULL"}, Category Object: {product.Category}");

            return new AdminProductViewModel
            {
                Id = product.Id,
                Name = product.Name ?? string.Empty,
                Description = product.Description ?? string.Empty,
                Price = product.Price,
                CategoryId = product.Category?.Id,
                Category = product.Category?.Name ?? "Без категория",
                SubcategoryId = product.SubCategory?.Id,
                Subcategory = product.SubCategory?.Name ?? "Без подкатегория",
                Color = product.Color?.Name ?? "Без цвят",
                ColorId = product.Color?.Id,
                Images = product.Images.Select(i => new AdminImageViewModel
                {
                    Id = i.Id,
                    Url = i.Url,
                    IsCover = i.IsCover
                }).ToList(),
                CoverImageUrl = product.CoverImageUrl,
                Sizes = MapViewModelSizes(product.Sizes),
                SizeIds = product.Sizes.Select(s => s.Id).ToList(),
                Quantity = product.Quantity
            };
        }

        public IEnumerable<AdminProductViewModel> MapAdminViewModelProducts(IEnumerable<ProductDto> products)
        {
            return (products ?? Enumerable.Empty<ProductDto>())
                .Select(MapAdminViewModelProduct)
                .ToList();
        }
    }
}
