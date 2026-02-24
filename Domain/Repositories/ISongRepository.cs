namespace SongsThatHelp.Domain.Repositories;

using SongsThatHelp.Domain.Entities;

public interface ISongRepository
{
    Song? GetById(int id);
    List<Song> GetAll(string? gangName);
    void Add(Song song);
    void Update(Song song);
    int GetNextId();
    int GetNextCommentId();
}
