using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SongsThatHelp.Application.Services;
using SongsThatHelp.Domain.Repositories;
using SongsThatHelp.Infrastructure.Data;
using SongsThatHelp.Infrastructure.Repositories;
using SongsThatHelp.Infrastructure.Services;
using SongsThatHelp.Presentation.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Configure PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? Environment.GetEnvironmentVariable("DATABASE_URL");

Console.WriteLine($"Connection string found: {!string.IsNullOrEmpty(connectionString)}");

if (!string.IsNullOrEmpty(connectionString))
{
    // Railway provides DATABASE_URL in a specific format, convert if needed
    if (connectionString.StartsWith("postgres://"))
    {
        Console.WriteLine("Converting postgres:// URL to Npgsql format");
        connectionString = connectionString.Replace("postgres://", "");
        var parts = connectionString.Split('@');
        var credentials = parts[0].Split(':');
        var hostAndDb = parts[1].Split('/');
        var hostAndPort = hostAndDb[0].Split(':');
        
        connectionString = $"Host={hostAndPort[0]};Port={hostAndPort[1]};Database={hostAndDb[1]};Username={credentials[0]};Password={credentials[1]};SSL Mode=Require;Trust Server Certificate=true";
        Console.WriteLine("Using PostgreSQL database");
    }
    
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));
    
    builder.Services.AddScoped<IUserRepository, EfUserRepository>();
    builder.Services.AddScoped<ISongRepository, EfSongRepository>();
}
else
{
    Console.WriteLine("No connection string found, using in-memory database");
    // Fallback to in-memory for local development
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("SongsDb"));
    
    builder.Services.AddScoped<IUserRepository, EfUserRepository>();
    builder.Services.AddScoped<ISongRepository, EfSongRepository>();
}

var secretKey = builder.Configuration["Jwt:SecretKey"] ?? "your-secret-key-min-32-chars-long!";
var issuer = builder.Configuration["Jwt:Issuer"] ?? "SongsThatHelp";

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configure JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = issuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

builder.Services.AddAuthorization();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Songs That Help API",
        Version = "v1",
        Description = "API for sharing songs with text, comments, and emoji reactions"
    });

    // Add JWT authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

// Register services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISongService, SongService>();

var app = builder.Build();

// Run migrations automatically (only for relational databases)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (db.Database.IsRelational())
    {
        db.Database.Migrate();
    }
    else
    {
        // For in-memory database, just ensure it's created
        db.Database.EnsureCreated();
    }
}

// Enable CORS
app.UseCors();

// Enable Swagger in all environments
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Songs That Help API v1");
    options.RoutePrefix = string.Empty; // Serve Swagger UI at root
});

app.UseAuthentication();
app.UseAuthorization();

// Map endpoints
app.MapAuthEndpoints();
app.MapSongEndpoints();

app.Run();
