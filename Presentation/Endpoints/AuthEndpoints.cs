namespace SongsThatHelp.Presentation.Endpoints;

using SongsThatHelp.Application.Services;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/api/register", (RegisterRequest req, IAuthService authService) =>
        {
            if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                return Results.BadRequest("Username and password required");
            
            return authService.Register(req.Username, req.Password)
                ? Results.Ok(new { message = "User registered" })
                : Results.BadRequest("Username already exists");
        })
        .WithName("Register")
        .WithTags("Authentication")
        .WithOpenApi(operation => 
        {
            operation.Summary = "Register a new user";
            operation.Description = "Create a new user account with username and password";
            return operation;
        });

        app.MapPost("/api/login", (LoginRequest req, IAuthService authService) =>
        {
            var token = authService.Login(req.Username, req.Password);
            return token != null
                ? Results.Ok(new { token, username = req.Username })
                : Results.Unauthorized();
        })
        .WithName("Login")
        .WithTags("Authentication")
        .WithOpenApi(operation => 
        {
            operation.Summary = "Login and get JWT token";
            operation.Description = "Authenticate with username and password to receive a JWT token for accessing protected endpoints";
            return operation;
        });
    }

    public record RegisterRequest(string Username, string Password);
    public record LoginRequest(string Username, string Password);
}
