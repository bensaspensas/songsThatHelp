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
var pgHost = Environment.GetEnvironmentVariable("PGHOST");
var pgPort = Environment.GetEnvironmentVariable("PGPORT");
var pgDatabase = Environment.GetEnvironmentVariable("PGDATABASE");
var pgUser = Environment.GetEnvironmentVariable("PGUSER");
var pgPassword = Environment.GetEnvironmentVariable("PGPASSWORD");

string? connectionString = null;

if (!string.IsNullOrEmpty(pgHost))
{
    Console.Error.WriteLine($"Building connection string with Host={pgHost}, Port={pgPort}, Database={pgDatabase}, User={pgUser}");
    
    // Build connection string with proper escaping
    var builder = new Npgsql.NpgsqlConnectionStringBuilder
    {
        Host = pgHost,
        Port = int.Parse(pgPort ?? "5432"),
        Database = pgDatabase,
        Username = pgUser,
        Password = pgPassword,
        SslMode = Npgsql.SslMode.Require,
        TrustServerCertificate = true
    };
    
    connectionString = builder.ConnectionString;
    Console.Error.WriteLine("Connection string built successfully");
}
else
{
    Console.Error.WriteLine("No Postgres connection found, using in-memory database");
}

if (!string.IsNullOrEmpty(connectionString))
{
    Console.Error.WriteLine("Configuring PostgreSQL");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));
    
    builder.Services.AddScoped<IUserRepository, EfUserRepository>();
    builder.Services.AddScoped<ISongRepository, EfSongRepository>();
}
else
{
    Console.Error.WriteLine("Configuring in-memory database");
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
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (db.Database.IsRelational())
        {
            Console.Error.WriteLine("Attempting to run migrations...");
            db.Database.Migrate();
            Console.Error.WriteLine("Migrations completed successfully");
        }
        else
        {
            // For in-memory database, just ensure it's created
            db.Database.EnsureCreated();
            Console.Error.WriteLine("In-memory database created");
        }
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Migration failed: {ex.Message}");
        Console.Error.WriteLine("App will continue without migrations. Database may need manual setup.");
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
