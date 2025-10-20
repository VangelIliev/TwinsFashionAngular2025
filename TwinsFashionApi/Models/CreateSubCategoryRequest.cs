using System;

namespace TwinsFashionApi.Models
{
    public class CreateSubCategoryRequest
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
