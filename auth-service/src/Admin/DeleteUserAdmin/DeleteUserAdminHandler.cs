using sevaLK_service_auth.Infra.Repositories;

namespace sevaLK_service_auth.Admin.DeleteUserAdmin;

public class DeleteUserAdminHandler
{
    private readonly IUserRepository _userRepository;

    public DeleteUserAdminHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(Guid userId, Guid currentUserId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
            throw new KeyNotFoundException("User not found");

        // Prevent admin from deleting themselves
        if (userId == currentUserId)
            throw new InvalidOperationException("Cannot delete your own account");

        // Prevent deleting other admin users
        if (user.Role == Domain.Enums.UserRole.Admin)
            throw new InvalidOperationException("Cannot delete an admin user");

        await _userRepository.DeleteAsync(userId);
    }
}
