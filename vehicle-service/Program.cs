using Microsoft.EntityFrameworkCore;
using vehicle_service.Infra;
using vehicle_service.Infra.Config;
using vehicle_service.Shared.Middlewares;

// Load environment variables from .env file
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        var instanceOrigin = Environment.GetEnvironmentVariable("CORS_ORIGIN");
        
        policy.WithOrigins(instanceOrigin, "http://localhost") // Allow both the specified origin and localhost for testing
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Infrastructure - Database, Repositories, Handlers, Security
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactApp");

// Add exception handler middleware (must be early in pipeline)
app.UseExceptionHandlerMiddleware();

// Add JWT middleware for token extraction
app.UseJwtMiddleware();

// Auto-create database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<VehicleDbContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/health", () => Results.Ok(new { status = "Vehicle Service is Healthy", time = DateTime.UtcNow }));

app.MapControllers();

app.Run();
