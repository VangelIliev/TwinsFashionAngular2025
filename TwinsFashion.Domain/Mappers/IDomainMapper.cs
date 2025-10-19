using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinsFashion.Data.Models;
using TwinsFashion.Domain.Models;

namespace TwinsFashion.Domain.Mappers
{
    public interface IDomainMapper
    {
        IEnumerable<ProductDto> MapDomainProducts(IEnumerable<Product> products);
        IEnumerable<SizeDto> MapDomainSizes(IEnumerable<Size> sizes);
        IEnumerable<ColorDto> MapDomainColors(IEnumerable<Color> colors);
        IEnumerable<CategoryDto> MapDomainCategories(IEnumerable<Category> categories);
        IEnumerable<SubCategoryDto> MapDomainSubCategories(IEnumerable<SubCategory> subcategories);
        ProductDto? MapDomainProductDto(Product? product);
    }
}
