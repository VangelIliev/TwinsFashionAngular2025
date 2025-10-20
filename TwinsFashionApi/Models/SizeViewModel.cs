namespace TwinsFashionApi.Models
{
    public class SizeViewModel
    {
        public Guid Id { get; set; }
        // Display text for size, e.g. "29", "39"
        public required string Size { get; set; }
        // Type, e.g. "Панталони", "Обувки"
        public required string Type { get; set; }
    }
}
