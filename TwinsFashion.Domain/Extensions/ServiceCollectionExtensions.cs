using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TwinsFashion.Data.Models;
using TwinsFashion.Domain.Implementation;
using TwinsFashion.Domain.Interfaces;
using TwinsFashion.Domain.Mappers;

namespace TwinsFashion.Domain.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTwinsFashionDomain(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IDomainMapper, DomainMapper>();

        return services;
    }
}
