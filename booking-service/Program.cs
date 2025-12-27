using Microsoft.EntityFrameworkCore;
using booking_service.Infra;
using booking_service.Infra.Config;
using booking_service.Shared.Middlewares;

// Load environment variables from .env file
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel();

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

// Add exception handler middleware (must be early in pipeline)
app.UseExceptionHandlerMiddleware();

// Add JWT middleware for token extraction
app.UseJwtMiddleware();

// Auto-create database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/health", () => Results.Ok(new { status = "Booking Service is Healthy", time = DateTime.UtcNow }));

app.MapControllers();

app.Run();
