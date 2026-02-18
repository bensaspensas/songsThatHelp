namespace SongsThatHelp.Domain.Entities;

public class User
{
    public string Username { get; private set; }
    public string PasswordHash { get; private set; }

    public User(string username, string passwordHash)
    {
        Username = username;
        PasswordHash = passwordHash;
    }
}
