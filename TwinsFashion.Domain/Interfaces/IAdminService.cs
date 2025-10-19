using System;
using System.Threading.Tasks;

namespace TwinsFashion.Domain.Interfaces
{
    public interface IAdminService
    {
        Task<bool> AuthoriseUser(string username, string password);
        Task<bool> RemoveProduct(Guid productId);
        Task<bool> RemoveProductSize(Guid productId, string size);
        Task<bool> CreateAdminUser(string username, string password);
    }
}
