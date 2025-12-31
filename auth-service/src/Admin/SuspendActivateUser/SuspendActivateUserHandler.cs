using sevaLK_service_auth.Infra.Repositories;

namespace sevaLK_service_auth.Admin.SuspendActivateUser;

public class SuspendActivateUserHandler
{
    private readonly IUserRepository _userRepository;

    public SuspendActivateUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<object> Handle(Guid userId, bool isActive)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
            throw new KeyNotFoundException("User not found");

        // Prevent admin from deactivating themselves
        if (!isActive && user.Role == Domain.Enums.UserRole.Admin)
            throw new InvalidOperationException("Cannot deactivate an admin user");

        if (isActive)
            user.Activate();
        else
            user.Deactivate();

        await _userRepository.UpdateAsync(user);

        return new
        {
            id = user.Id,
            isActive = user.IsActive
        };
    }
}
