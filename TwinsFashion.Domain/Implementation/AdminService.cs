using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using TwinsFashion.Data.Models;
using TwinsFashion.Domain.Interfaces;

namespace TwinsFashion.Domain.Implementation
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _context;

        public AdminService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AuthoriseUser(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
            {
                return false;
            }

            var user = await _context.AdminUsers.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return false;
            }

            var hash = HashPassword(password, user.PasswordSalt);
            return hash == user.PasswordHash;
        }

        public async Task<bool> RemoveProduct(Guid productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return false;
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveProductSize(Guid productId, string size)
        {
            if (string.IsNullOrWhiteSpace(size))
            {
                return false;
            }

            var product = await _context.Products
                .Include(p => p.Sizes)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                return false;
            }

            var sizeEntity = product.Sizes.FirstOrDefault(s => s.Name == size);
            if (sizeEntity == null)
            {
                return false;
            }

            product.Sizes.Remove(sizeEntity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CreateAdminUser(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            var exists = await _context.AdminUsers.AnyAsync(u => u.Username == username);
            if (exists)
            {
                return true;
            }

            var salt = GenerateSalt();
            var hash = HashPassword(password, salt);

            var user = new AdminUser
            {
                Id = Guid.NewGuid(),
                Username = username,
                PasswordSalt = salt,
                PasswordHash = hash
            };

            _context.AdminUsers.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }

        private static string HashPassword(string password, string salt)
        {
            using var sha256 = SHA256.Create();
            var salted = Encoding.UTF8.GetBytes(password + salt);
            var hash = sha256.ComputeHash(salted);
            return Convert.ToBase64String(hash);
        }

        private static string GenerateSalt(int size = 16)
        {
            var bytes = new byte[size];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
    
