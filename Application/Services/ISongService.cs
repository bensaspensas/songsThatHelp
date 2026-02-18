namespace SongsThatHelp.Application.Services;

using SongsThatHelp.Domain.Entities;

public interface ISongService
{
    Song CreateSong(string username, string link, string text);
    Song? GetSong(int id);
    List<Song> GetAllSongs();
    Comment? AddComment(int songId, string username, string text);
    bool AddEmoji(int songId, string username, string emojiType);
}
