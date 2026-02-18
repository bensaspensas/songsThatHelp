namespace SongsThatHelp.Application.Services;

using SongsThatHelp.Domain.Entities;
using SongsThatHelp.Domain.Repositories;

public class SongService : ISongService
{
    private readonly ISongRepository _songRepository;

    public SongService(ISongRepository songRepository)
    {
        _songRepository = songRepository;
    }

    public Song CreateSong(string username, string link, string text)
    {
        var id = _songRepository.GetNextId();
        var song = new Song(id, username, link, text);
        _songRepository.Add(song);
        return song;
    }

    public Song? GetSong(int id)
    {
        return _songRepository.GetById(id);
    }

    public List<Song> GetAllSongs()
    {
        return _songRepository.GetAll();
    }

    public Comment? AddComment(int songId, string username, string text)
    {
        var song = _songRepository.GetById(songId);
        if (song == null) return null;

        var commentId = _songRepository.GetNextCommentId();
        return song.AddComment(commentId, username, text);
    }

    public bool AddEmoji(int songId, string username, string emojiType)
    {
        var song = _songRepository.GetById(songId);
        if (song == null) return false;

        song.AddEmoji(username, emojiType);
        return true;
    }
}
