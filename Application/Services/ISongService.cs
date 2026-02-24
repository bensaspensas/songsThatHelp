namespace SongsThatHelp.Application.Services;

using SongsThatHelp.Domain.Entities;

public interface ISongService
{
    Song CreateSong(string username, string link, string text, string? gangName);
    Song? GetSong(int id, string? gangName);
    List<Song> GetAllSongs(string? gangName);
    Comment? AddComment(int songId, string username, string text, string? gangName);
    bool AddEmoji(int songId, string username, string emojiType, string? gangName);
}
