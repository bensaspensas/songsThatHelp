namespace SongsThatHelp.Infrastructure.Repositories;

using SongsThatHelp.Domain.Entities;
using SongsThatHelp.Domain.Repositories;

public class InMemorySongRepository : ISongRepository
{
    private readonly List<Song> _songs = new();
    private int _nextSongId = 1;
    private int _nextCommentId = 1;

    public Song? GetById(int id)
    {
        return _songs.FirstOrDefault(s => s.Id == id);
    }

    public List<Song> GetAll()
    {
        return _songs;
    }

    public void Add(Song song)
    {
        _songs.Add(song);
    }

    public void Update(Song song)
    {
        // In-memory doesn't need explicit update since objects are already in memory
    }

    public int GetNextId()
    {
        return _nextSongId++;
    }

    public int GetNextCommentId()
    {
        return _nextCommentId++;
    }
}
