using sevaLK_service_auth.Domain.Entities;
using sevaLK_service_auth.Domain.Enums;

namespace sevaLK_service_auth.Infra.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task<(IEnumerable<User> Users, int Total)> GetAllPaginatedAsync(int page, int limit, string? search = null, UserRole? role = null);
    Task<User> AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(Guid id);
    Task<bool> EmailExistsAsync(string email);
}
