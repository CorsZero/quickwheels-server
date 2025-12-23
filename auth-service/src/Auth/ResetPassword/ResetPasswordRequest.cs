namespace sevaLK_service_auth.Auth.ResetPassword;

public record RequestPasswordResetRequest(
    string Email
);

public record ResetPasswordRequest(
    string Email,
    string Token,
    string NewPassword,
    string ConfirmPassword
);
