using Microsoft.EntityFrameworkCore;
using sevaLK_service_auth.Auth.ChangePassword;
using sevaLK_service_auth.Auth.Login;
using sevaLK_service_auth.Auth.Logout;
using sevaLK_service_auth.Auth.RefreshToken;
using sevaLK_service_auth.Auth.Register;
using sevaLK_service_auth.Auth.ResetPassword;
using sevaLK_service_auth.Auth.Profile;
using sevaLK_service_auth.Admin.GetAllUsersAdmin;
using sevaLK_service_auth.Admin.GetUserByIdAdmin;
using sevaLK_service_auth.Admin.SuspendActivateUser;
using sevaLK_service_auth.Admin.DeleteUserAdmin;
using sevaLK_service_auth.Infra;
using sevaLK_service_auth.Infra.Config;
using sevaLK_service_auth.Shared.Middlewares;

// Load environment variables from .env file
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Infrastructure - Database, Repositories, Security
builder.Services.AddInfrastructure(builder.Configuration);

// Feature Handlers
builder.Services.AddScoped<RegisterHandler>();
builder.Services.AddScoped<LoginHandler>();
builder.Services.AddScoped<LogoutHandler>();
builder.Services.AddScoped<RefreshTokenHandler>();
builder.Services.AddScoped<ChangePasswordHandler>();
builder.Services.AddScoped<ResetPasswordHandler>();
// Profile handlers
builder.Services.AddScoped<GetCurrentProfileHandler>();
builder.Services.AddScoped<UpdateProfileHandler>();

// Admin Handlers
builder.Services.AddScoped<GetAllUsersAdminHandler>();
builder.Services.AddScoped<GetUserByIdAdminHandler>();
builder.Services.AddScoped<SuspendActivateUserHandler>();
builder.Services.AddScoped<DeleteUserAdminHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Add exception handler middleware (must be early in pipeline)
app.UseExceptionHandlerMiddleware();

// Add JWT middleware for token extraction
app.UseJwtMiddleware();

// Auto-create database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/health", () => Results.Ok(new { status = "Server is Healthy", time = DateTime.UtcNow }));

app.MapControllers();

app.Run();