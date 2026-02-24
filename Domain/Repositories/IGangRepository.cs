namespace SongsThatHelp.Domain.Repositories;

using SongsThatHelp.Domain.Entities;

public interface IGangRepository
{
    Gang? GetByName(string name);
    void Add(Gang gang);
    bool Exists(string name);
}
