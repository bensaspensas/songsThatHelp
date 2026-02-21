namespace SongsThatHelp.Domain.Entities;

public class Song
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    private readonly List<Comment> _comments = new();
    private readonly List<Emoji> _emojis = new();

    public IReadOnlyList<Comment> Comments => _comments.AsReadOnly();
    public IReadOnlyList<Emoji> Emojis => _emojis.AsReadOnly();

    public Song() 
    {
        CreatedAt = DateTime.UtcNow;
    }

    public Song(int id, string username, string link, string text)
    {
        Id = id;
        Username = username;
        Link = link;
        Text = text;
        CreatedAt = DateTime.UtcNow;
    }

    public Comment AddComment(int commentId, string username, string text)
    {
        var comment = new Comment(commentId, username, text);
        _comments.Add(comment);
        return comment;
    }

    public void AddEmoji(string username, string emojiType)
    {
        _emojis.RemoveAll(e => e.Username == username);
        _emojis.Add(new Emoji(username, emojiType));
    }
}

public class Comment
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public Comment()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public Comment(int id, string username, string text)
    {
        Id = id;
        Username = username;
        Text = text;
        CreatedAt = DateTime.UtcNow;
    }
}

public class Emoji
{
    public string Username { get; set; } = string.Empty;
    public string EmojiType { get; set; } = string.Empty;

    public Emoji() { }

    public Emoji(string username, string emojiType)
    {
        Username = username;
        EmojiType = emojiType;
    }
}
