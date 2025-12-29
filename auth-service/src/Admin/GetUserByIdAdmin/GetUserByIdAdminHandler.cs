using sevaLK_service_auth.Infra.Repositories;

namespace sevaLK_service_auth.Admin.GetUserByIdAdmin;

public class GetUserByIdAdminHandler
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdAdminHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<object> Handle(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
            throw new KeyNotFoundException("User not found");

        return new
        {
            id = user.Id,
            email = user.Email,
            fullName = user.FullName,
            phone = user.Phone,
            role = user.Role.ToString().ToUpper(),
            isActive = user.IsActive,
            createdAt = user.CreatedAt
        };
    }
}
