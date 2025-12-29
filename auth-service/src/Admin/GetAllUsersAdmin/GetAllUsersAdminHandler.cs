using sevaLK_service_auth.Domain.Enums;
using sevaLK_service_auth.Infra.Repositories;

namespace sevaLK_service_auth.Admin.GetAllUsersAdmin;

public class GetAllUsersAdminHandler
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersAdminHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<object> Handle(int page, int limit, string? search, string? role)
    {
        UserRole? roleEnum = null;
        if (!string.IsNullOrWhiteSpace(role))
        {
            if (Enum.TryParse<UserRole>(role, true, out var parsedRole))
            {
                roleEnum = parsedRole;
            }
        }

        var (users, total) = await _userRepository.GetAllPaginatedAsync(page, limit, search, roleEnum);

        var userResponses = users.Select(u => new
        {
            id = u.Id,
            email = u.Email,
            fullName = u.FullName,
            phone = u.Phone,
            role = u.Role.ToString().ToUpper(),
            isActive = u.IsActive,
            createdAt = u.CreatedAt
        }).ToList();

        return new
        {
            users = userResponses,
            pagination = new
            {
                page,
                limit,
                total,
                totalPages = (int)Math.Ceiling((double)total / limit)
            }
        };
    }
}
