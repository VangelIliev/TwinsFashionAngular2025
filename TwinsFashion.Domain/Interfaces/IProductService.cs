using TwinsFashion.Domain.Models;

namespace TwinsFashion.Domain.Interfaces
{
    public interface IProductService
    {
        public Task<IEnumerable<ProductDto>> GetAllProductsAsync();

        public Task<ProductDto?> GetProductByIdAsync(Guid id);
        Task<bool> SeedProductToDatabase(string categoryName, string colorName, string subcategoryName, IEnumerable<SizeDto> sizes);

        Task<IEnumerable<ColorDto>> GetColors();

        Task<IEnumerable<CategoryDto>> GetCategories();

        Task<IEnumerable<SizeDto>> GetSizes();

        Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(string categoryName);

        Task<IEnumerable<ProductDto>> GetProductsByColorAsync(string colorName);

        // Expose subcategories for UI selections
        Task<IEnumerable<SubCategoryDto>> GetSubCategories();

        // Creates a new product with related entities
        Task<bool> AddProductInDatabase(
            string name,
            string description,
            int price,
            int quantity,
            Guid categoryId,
            Guid subCategoryId,
            Guid colorId,
            IEnumerable<string> imageUrls,
            IEnumerable<Guid> sizeIds);

        // Admin create operations
        Task<bool> CreateCategoryAsync(string name);
        Task<bool> CreateColorAsync(string name);
        Task<bool> CreateSizeAsync(string type, string size);
        Task<bool> CreateSubCategoryAsync(Guid categoryId, string name);
        
        // Set cover image for product
        Task<bool> SetCoverImageAsync(Guid productId, Guid imageId);
    }
}
