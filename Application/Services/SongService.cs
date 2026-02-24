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

    public Song CreateSong(string username, string link, string text, string? gangName)
    {
        var id = _songRepository.GetNextId();
        var song = new Song(id, username, link, text, gangName);
        _songRepository.Add(song);
        return song;
    }

    public Song? GetSong(int id, string? gangName)
    {
        var song = _songRepository.GetById(id);
        if (song == null) return null;
        
        // Check gang scope
        if (gangName != null && song.GangName != gangName)
            return null;
        if (gangName == null && song.GangName != null)
            return null;
            
        return song;
    }

    public List<Song> GetAllSongs(string? gangName)
    {
        return _songRepository.GetAll(gangName);
    }

    public Comment? AddComment(int songId, string username, string text, string? gangName)
    {
        var song = GetSong(songId, gangName);
        if (song == null) return null;

        var commentId = _songRepository.GetNextCommentId();
        var comment = song.AddComment(commentId, username, text);
        _songRepository.Update(song);
        return comment;
    }

    public bool AddEmoji(int songId, string username, string emojiType, string? gangName)
    {
        var song = GetSong(songId, gangName);
        if (song == null) return false;

        song.AddEmoji(username, emojiType);
        _songRepository.Update(song);
        return true;
    }
}
