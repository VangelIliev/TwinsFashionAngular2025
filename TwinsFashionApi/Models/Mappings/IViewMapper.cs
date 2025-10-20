using TwinsFashion.Domain.Models;

namespace TwinsFashionApi.Models.Mappings
{
    public interface IViewMapper
    {
        IEnumerable<ProductViewModel> MapViewModelProducts(IEnumerable<ProductDto> products);
        ProductViewModel MapViewModelProduct(ProductDto? product);
        List<string> MapViewModelSizes(IEnumerable<SizeDto> sizes);
        IEnumerable<ColorViewModel> MapViewModelColors(IEnumerable<ColorDto> colors);
        IEnumerable<CategoryViewModel> MapViewModelCategories(IEnumerable<CategoryDto> categories);
        IEnumerable<SubCategoryViewModel> MapViewModelSubCategories(IEnumerable<SubCategoryDto> subcategories);
        
        // Admin methods
        IEnumerable<AdminProductViewModel> MapAdminViewModelProducts(IEnumerable<ProductDto> products);
        AdminProductViewModel MapAdminViewModelProduct(ProductDto? product);
    }
}
