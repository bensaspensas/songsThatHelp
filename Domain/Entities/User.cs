namespace SongsThatHelp.Domain.Entities;

public class User
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? GangName { get; set; }

    public User() { }

    public User(string username, string passwordHash, string? gangName = null)
    {
        Username = username;
        PasswordHash = passwordHash;
        GangName = gangName;
    }
}

