using Microsoft.EntityFrameworkCore;
using EmptyLegs.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database configuration - use InMemory for development
builder.Services.AddDbContext<EmptyLegsDbContext>(options =>
{
    options.UseInMemoryDatabase("EmptyLegsInMemoryDb");
});

// CORS Configuration - simple version
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseRouting();
app.MapControllers();

// Simple health check endpoint
app.MapGet("/health", () => "Healthy");

// Test endpoint
app.MapGet("/", () => "Empty Legs API is running!");

app.Run();