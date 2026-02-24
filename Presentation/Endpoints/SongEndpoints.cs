namespace SongsThatHelp.Presentation.Endpoints;

using SongsThatHelp.Application.Services;
using System.Security.Claims;

public static class SongEndpoints
{
    public static void MapSongEndpoints(this WebApplication app)
    {
        var songGroup = app.MapGroup("/api/songs")
            .RequireAuthorization()
            .WithTags("Songs");

        songGroup.MapGet("", (ClaimsPrincipal user, ISongService songService) =>
        {
            var gangName = user.FindFirst("gang")?.Value;
            return Results.Ok(songService.GetAllSongs(gangName));
        })
        .WithName("GetAllSongs")
        .WithOpenApi(operation => 
        {
            operation.Summary = "Get all songs";
            operation.Description = "Retrieve songs scoped to your gang (or global if no gang)";
            return operation;
        });

        songGroup.MapGet("{id}", (int id, ClaimsPrincipal user, ISongService songService) =>
        {
            var gangName = user.FindFirst("gang")?.Value;
            var song = songService.GetSong(id, gangName);
            return song != null ? Results.Ok(song) : Results.NotFound();
        })
        .WithName("GetSongById")
        .WithOpenApi(operation => 
        {
            operation.Summary = "Get song by ID";
            operation.Description = "Retrieve a specific song (must be in your gang scope)";
            return operation;
        });

        songGroup.MapPost("", (PostSongRequest req, ClaimsPrincipal user, ISongService songService) =>
        {
            var username = user.Identity?.Name;
            var gangName = user.FindFirst("gang")?.Value;
            
            if (string.IsNullOrWhiteSpace(username))
                return Results.Unauthorized();

            if (string.IsNullOrWhiteSpace(req.Link))
                return Results.BadRequest("Link required");
            
            var song = songService.CreateSong(username, req.Link, req.Text ?? "", gangName);
            return Results.Created($"/api/songs/{song.Id}", song);
        })
        .WithName("PostSong")
        .WithOpenApi(operation => 
        {
            operation.Summary = "Post a new song";
            operation.Description = "Share a new song (scoped to your gang if you're in one)";
            return operation;
        });

        songGroup.MapPost("{id}/comments", (int id, AddCommentRequest req, ClaimsPrincipal user, ISongService songService) =>
        {
            var username = user.Identity?.Name;
            var gangName = user.FindFirst("gang")?.Value;
            
            if (string.IsNullOrWhiteSpace(username))
                return Results.Unauthorized();

            var comment = songService.AddComment(id, username, req.Text, gangName);
            return comment != null ? Results.Ok(comment) : Results.NotFound();
        })
        .WithName("AddComment")
        .WithOpenApi(operation => 
        {
            operation.Summary = "Add a comment to a song";
            operation.Description = "Post a comment on a song (must be in same gang scope)";
            return operation;
        });

        songGroup.MapPost("{id}/emojis", (int id, AddEmojiRequest req, ClaimsPrincipal user, ISongService songService) =>
        {
            var username = user.Identity?.Name;
            var gangName = user.FindFirst("gang")?.Value;
            
            if (string.IsNullOrWhiteSpace(username))
                return Results.Unauthorized();

            return songService.AddEmoji(id, username, req.EmojiType, gangName)
                ? Results.Ok(new { message = "Emoji added" })
                : Results.NotFound();
        })
        .WithName("AddEmoji")
        .WithOpenApi(operation => 
        {
            operation.Summary = "Add or update emoji reaction";
            operation.Description = "React to a song with an emoji (must be in same gang scope)";
            return operation;
        });
    }

    public record PostSongRequest(string Link, string? Text);
    public record AddCommentRequest(string Text);
    public record AddEmojiRequest(string EmojiType);
}
