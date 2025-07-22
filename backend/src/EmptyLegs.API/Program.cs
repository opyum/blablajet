using EmptyLegs.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using Microsoft.OpenApi.Models;
using AspNetCoreRateLimit;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/empty-legs-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger Configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Empty Legs API", 
        Version = "v1",
        Description = "API for Empty Legs platform - Private jet empty leg flight management"
    });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Database Configuration
if (builder.Environment.IsDevelopment() && builder.Configuration.GetValue<bool>("UseInMemoryDatabase", false))
{
    builder.Services.AddDbContext<EmptyLegsDbContext>(options =>
        options.UseInMemoryDatabase("EmptyLegsInMemory"));
}
else
{
    builder.Services.AddDbContext<EmptyLegsDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
}

// Redis Configuration
if (!builder.Environment.IsDevelopment() || !builder.Configuration.GetValue<bool>("UseInMemoryDatabase", false))
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetConnectionString("Redis");
    });
}
else
{
    builder.Services.AddMemoryCache();
}

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JWT");
var secretKey = Encoding.ASCII.GetBytes(jwtSettings["Secret"] ?? "your-super-secret-key-here");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Rate Limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

// SignalR for real-time notifications
builder.Services.AddSignalR();

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program), typeof(EmptyLegs.Application.Mappings.MappingProfile));

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Dependency Injection
builder.Services.AddScoped<EmptyLegs.Core.Interfaces.IUnitOfWork, EmptyLegs.Infrastructure.Repositories.UnitOfWork>();
builder.Services.AddScoped<EmptyLegs.Core.Interfaces.IJwtTokenService, EmptyLegs.Application.Services.JwtTokenService>();
builder.Services.AddScoped<EmptyLegs.Application.Interfaces.IAuthService, EmptyLegs.Application.Services.AuthService>();
builder.Services.AddHttpContextAccessor();

// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<EmptyLegsDbContext>();
    // .AddRedis(builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379"); // Désactivé temporairement

var app = builder.Build();

// Database initialization
if (app.Environment.IsDevelopment() && app.Configuration.GetValue<bool>("UseInMemoryDatabase", false))
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<EmptyLegsDbContext>();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Empty Legs API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

// Rate Limiting
app.UseIpRateLimiting();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Request Logging
app.UseSerilogRequestLogging();

// Health Check endpoint
app.MapHealthChecks("/health");

// Controllers
app.MapControllers();

// SignalR Hubs
// app.MapHub<NotificationHub>("/hubs/notifications");

// Global Exception Handler
app.UseExceptionHandler("/error");

Log.Information("Starting Empty Legs API...");

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// Make Program class accessible for integration tests
public partial class Program { }