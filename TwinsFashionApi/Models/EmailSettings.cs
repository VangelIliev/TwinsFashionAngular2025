namespace TwinsFashionApi.Models
{
    public class EmailSettings
    {
        public required string Host { get; set; }
        public required int Port { get; set; }
        public required bool EnableSSL { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }
}
