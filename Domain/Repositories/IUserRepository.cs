namespace SongsThatHelp.Domain.Repositories;

using SongsThatHelp.Domain.Entities;

public interface IUserRepository
{
    User? GetByUsername(string username);
    void Add(User user);
    bool Exists(string username);
}
