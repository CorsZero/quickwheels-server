namespace sevaLK_service_auth.Auth.Profile;

public class UpdateProfileRequest
{
    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public IFormFile? ProfileImage { get; set; }
}
