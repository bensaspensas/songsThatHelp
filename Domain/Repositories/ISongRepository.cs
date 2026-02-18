namespace SongsThatHelp.Domain.Repositories;

using SongsThatHelp.Domain.Entities;

public interface ISongRepository
{
    Song? GetById(int id);
    List<Song> GetAll();
    void Add(Song song);
    int GetNextId();
    int GetNextCommentId();
}
