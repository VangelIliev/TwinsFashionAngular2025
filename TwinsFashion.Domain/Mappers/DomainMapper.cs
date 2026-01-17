using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TwinsFashion.Data.Models;
using TwinsFashion.Domain.Models;

namespace TwinsFashion.Domain.Mappers
{
    public class DomainMapper : IDomainMapper
    {
        public IEnumerable<CategoryDto> MapDomainCategories(IEnumerable<Category> categories)
        {
            if (categories == null || !categories.Any())
            {
                return Enumerable.Empty<CategoryDto>();
            }

            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name
            });
        }

        public IEnumerable<ColorDto> MapDomainColors(IEnumerable<Color> colors)
        {
            if (colors == null || !colors.Any())
            {
                return Enumerable.Empty<ColorDto>();
            }

            return colors.Select(c => new ColorDto
            {
                Id = c.Id,
                Name = c.Name
            });
        }

        public IEnumerable<ProductDto> MapDomainProducts(IEnumerable<Product> products)
        {
            if (products == null || !products.Any())
            {
                return Enumerable.Empty<ProductDto>();
            }

            return products.Select(MapDomainProductDto).Where(p => p is not null)!;
        }

        public IEnumerable<SizeDto> MapDomainSizes(IEnumerable<Size> sizes)
        {
            if (sizes == null || !sizes.Any())
            {
                return Enumerable.Empty<SizeDto>();
            }

            return sizes.Select(MapSizeDto).Where(dto => dto is not null)!;
        }

        public IEnumerable<SubCategoryDto> MapDomainSubCategories(IEnumerable<SubCategory> subcategories)
        {
            if (subcategories == null || !subcategories.Any())
            {
                return Enumerable.Empty<SubCategoryDto>();
            }

            return subcategories.Select(sc => new SubCategoryDto
            {
                Id = sc.Id,
                Name = sc.Name,
                CategoryId = sc.CategoryId,
                Category = sc.Category is null ? null : new CategoryDto
                {
                    Id = sc.Category.Id,
                    Name = sc.Category.Name
                }
            });
        }

        public ProductDto? MapDomainProductDto(Product? product)
        {
            if (product == null)
            {
                return null;
            }

            var images = product.Images ?? [];
            var coverImageUrl = images.FirstOrDefault(img => img.IsCover)?.Url ?? images.FirstOrDefault()?.Url ?? string.Empty;
            coverImageUrl = WithAutoFormat(coverImageUrl);

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                LongDescription = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                Badge = string.Empty,
                CoverImageUrl = coverImageUrl,
                Category = product.Category is null ? null : new CategoryDto
                {
                    Id = product.Category.Id,
                    Name = product.Category.Name
                },
                SubCategory = product.SubCategory is null ? null : new SubCategoryDto
                {
                    Id = product.SubCategory.Id,
                    Name = product.SubCategory.Name,
                    CategoryId = product.SubCategory.CategoryId
                },
                Color = product.Color is null ? null : new ColorDto
                {
                    Id = product.Color.Id,
                    Name = product.Color.Name
                },
                Images = images.Select(img => new ImageDto
                {
                    Id = img.Id,
                    Url = WithAutoFormat(img.Url),
                    Alt = string.IsNullOrWhiteSpace(img.Url) ? product.Name : product.Name,
                    IsCover = img.IsCover
                }).ToList(),
                Sizes = MapDomainSizes(product.Sizes ?? []).ToList()
            };
        }

        private static SizeDto? MapSizeDto(Size size)
        {
            if (size == null || string.IsNullOrWhiteSpace(size.Name))
            {
                return null;
            }

            try
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(size.Name) ?? new();
                return new SizeDto
                {
                    Id = size.Id,
                    Type = dict.TryGetValue("type", out var type) ? type : string.Empty,
                    Size = dict.TryGetValue("size", out var s) ? s : string.Empty
                };
            }
            catch (JsonException)
            {
                return new SizeDto
                {
                    Id = size.Id,
                    Type = string.Empty,
                    Size = size.Name
                };
            }
        }

        private static string WithAutoFormat(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return string.Empty;
            }

            const string uploadSegment = "/upload/";
            if (!url.Contains("res.cloudinary.com", StringComparison.OrdinalIgnoreCase) ||
                !url.Contains(uploadSegment, StringComparison.OrdinalIgnoreCase))
            {
                return url;
            }

            if (url.Contains("/upload/f_auto", StringComparison.OrdinalIgnoreCase) ||
                url.Contains("/upload/q_auto", StringComparison.OrdinalIgnoreCase))
            {
                return url;
            }

            return url.Replace(uploadSegment, "/upload/f_auto,q_auto/", StringComparison.OrdinalIgnoreCase);
        }

    }
}
