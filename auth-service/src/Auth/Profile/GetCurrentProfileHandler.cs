using sevaLK_service_auth.Infra.Repositories;
using sevaLK_service_auth.Shared.Middlewares;

namespace sevaLK_service_auth.Auth.Profile;

public class GetCurrentProfileHandler
{
    private readonly IUserRepository _userRepository;

    public GetCurrentProfileHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ApiResponse> Handle(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return ApiResponse.ErrorResult("User not found");
        }

        return ApiResponse.SuccessResult(new
        {
            user.Id,
            user.Email,
            user.FullName,
            user.Phone,
            Role = user.Role.ToString(),
            user.IsActive,
            user.CreatedAt,
            user.UpdatedAt,
            user.LastLoginAt
        });
    }
}
