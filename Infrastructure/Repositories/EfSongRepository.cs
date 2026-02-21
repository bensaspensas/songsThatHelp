namespace SongsThatHelp.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using SongsThatHelp.Domain.Entities;
using SongsThatHelp.Domain.Repositories;
using SongsThatHelp.Infrastructure.Data;

public class EfSongRepository : ISongRepository
{
    private readonly AppDbContext _context;

    public EfSongRepository(AppDbContext context)
    {
        _context = context;
    }

    public Song? GetById(int id)
    {
        return _context.Songs.Find(id);
    }

    public List<Song> GetAll()
    {
        return _context.Songs.ToList();
    }

    public void Add(Song song)
    {
        _context.Songs.Add(song);
        _context.SaveChanges();
    }

    public int GetNextId()
    {
        return _context.Songs.Any() ? _context.Songs.Max(s => s.Id) + 1 : 1;
    }

    public int GetNextCommentId()
    {
        // For simplicity, using a global counter across all songs
        var maxId = 0;
        foreach (var song in _context.Songs)
        {
            if (song.Comments.Any())
            {
                var songMaxId = song.Comments.Max(c => c.Id);
                if (songMaxId > maxId) maxId = songMaxId;
            }
        }
        return maxId + 1;
    }
}
