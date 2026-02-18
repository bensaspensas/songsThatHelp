namespace SongsThatHelp.Presentation.Endpoints;

using SongsThatHelp.Application.Services;
using System.Security.Claims;

public static class SongEndpoints
{
    public static void MapSongEndpoints(this WebApplication app)
    {
        var songGroup = app.MapGroup("/api/songs")
            //.RequireAuthorization()
            .WithTags("Songs");

        songGroup.MapGet("", (ISongService songService) => 
            Results.Ok(songService.GetAllSongs()))
            .WithName("GetAllSongs")
            .WithOpenApi(operation => 
            {
                operation.Summary = "Get all songs";
                operation.Description = "Retrieve a list of all posted songs with their comments and emojis";
                return operation;
            });

        songGroup.MapGet("{id}", (int id, ISongService songService) =>
        {
            var song = songService.GetSong(id);
            return song != null ? Results.Ok(song) : Results.NotFound();
        })
        .WithName("GetSongById")
        .WithOpenApi(operation => 
        {
            operation.Summary = "Get song by ID";
            operation.Description = "Retrieve a specific song with all its comments and emoji reactions";
            return operation;
        });

        songGroup.MapPost("", (PostSongRequest req, ClaimsPrincipal user, ISongService songService) =>
        {
            var username = user.Identity?.Name;
            if (string.IsNullOrWhiteSpace(username))
                return Results.Unauthorized();

            if (string.IsNullOrWhiteSpace(req.Link))
                return Results.BadRequest("Link required");
            
            var song = songService.CreateSong(username, req.Link, req.Text ?? "");
            return Results.Created($"/api/songs/{song.Id}", song);
        })
        .WithName("PostSong")
        .WithOpenApi(operation => 
        {
            operation.Summary = "Post a new song";
            operation.Description = "Share a new song with a link and optional text description";
            return operation;
        });

        songGroup.MapPost("{id}/comments", (int id, AddCommentRequest req, ClaimsPrincipal user, ISongService songService) =>
        {
            var username = user.Identity?.Name;
            if (string.IsNullOrWhiteSpace(username))
                return Results.Unauthorized();

            var comment = songService.AddComment(id, username, req.Text);
            return comment != null ? Results.Ok(comment) : Results.NotFound();
        })
        .WithName("AddComment")
        .WithOpenApi(operation => 
        {
            operation.Summary = "Add a comment to a song";
            operation.Description = "Post a comment on a specific song";
            return operation;
        });

        songGroup.MapPost("{id}/emojis", (int id, AddEmojiRequest req, ClaimsPrincipal user, ISongService songService) =>
        {
            var username = user.Identity?.Name;
            if (string.IsNullOrWhiteSpace(username))
                return Results.Unauthorized();

            return songService.AddEmoji(id, username, req.EmojiType)
                ? Results.Ok(new { message = "Emoji added" })
                : Results.NotFound();
        })
        .WithName("AddEmoji")
        .WithOpenApi(operation => 
        {
            operation.Summary = "Add or update emoji reaction";
            operation.Description = "React to a song with an emoji. Only one emoji per user per song (replaces previous emoji)";
            return operation;
        });
    }

    public record PostSongRequest(string Link, string? Text);
    public record AddCommentRequest(string Text);
    public record AddEmojiRequest(string EmojiType);
}
