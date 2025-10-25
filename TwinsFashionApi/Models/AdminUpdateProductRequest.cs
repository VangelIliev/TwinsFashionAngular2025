using System;
using System.Collections.Generic;

namespace TwinsFashionApi.Models
{
    public class AdminUpdateProductRequest
    {
        public string? Name { get; set; }
        public int? Price { get; set; }
        public Guid? SubCategoryId { get; set; }
        public List<Guid>? SizeIds { get; set; }
    }
}
