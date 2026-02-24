namespace SongsThatHelp.Presentation.Endpoints;

using SongsThatHelp.Application.Services;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/api/gangs", (CreateGangRequest req, IAuthService authService) =>
        {
            if (string.IsNullOrWhiteSpace(req.GangName) || string.IsNullOrWhiteSpace(req.GangPassword))
                return Results.BadRequest("Gang name and password required");
            
            return authService.CreateGang(req.GangName, req.GangPassword)
                ? Results.Ok(new { message = "Gang created" })
                : Results.BadRequest("Gang already exists");
        })
        .WithName("CreateGang")
        .WithTags("Authentication")
        .WithOpenApi(operation => 
        {
            operation.Summary = "Create a new gang";
            operation.Description = "Create a gang that users can join";
            return operation;
        });

        app.MapPost("/api/register", (RegisterRequest req, IAuthService authService) =>
        {
            if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                return Results.BadRequest("Username and password required");
            
            return authService.Register(req.Username, req.Password, req.GangName, req.GangPassword)
                ? Results.Ok(new { message = "User registered" })
                : Results.BadRequest("Username already exists or invalid gang credentials");
        })
        .WithName("Register")
        .WithTags("Authentication")
        .WithOpenApi(operation => 
        {
            operation.Summary = "Register a new user";
            operation.Description = "Create a new user account with username and password, optionally join a gang";
            return operation;
        });

        app.MapPost("/api/login", (LoginRequest req, IAuthService authService) =>
        {
            var (token, gangName) = authService.Login(req.Username, req.Password, req.GangName, req.GangPassword);
            return token != null
                ? Results.Ok(new { token, username = req.Username, gangName })
                : Results.Unauthorized();
        })
        .WithName("Login")
        .WithTags("Authentication")
        .WithOpenApi(operation => 
        {
            operation.Summary = "Login and get JWT token";
            operation.Description = "Authenticate with username and password, optionally specify gang to join/switch";
            return operation;
        });
    }

    public record CreateGangRequest(string GangName, string GangPassword);
    public record RegisterRequest(string Username, string Password, string? GangName = null, string? GangPassword = null);
    public record LoginRequest(string Username, string Password, string? GangName = null, string? GangPassword = null);
}
