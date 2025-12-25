var builder = WebApplication.CreateBuilder(args);



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/health", () => Results.Ok(new { status = "Booking Service is Healthy", time = DateTime.UtcNow }));


app.Run();
