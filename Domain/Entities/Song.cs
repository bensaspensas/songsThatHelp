namespace SongsThatHelp.Domain.Entities;

public class Song
{
    public int Id { get; private set; }
    public string Username { get; private set; }
    public string Link { get; private set; }
    public string Text { get; private set; }
    public DateTime CreatedAt { get; private set; }
    private readonly List<Comment> _comments = new();
    private readonly List<Emoji> _emojis = new();

    public IReadOnlyList<Comment> Comments => _comments.AsReadOnly();
    public IReadOnlyList<Emoji> Emojis => _emojis.AsReadOnly();

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
    public int Id { get; private set; }
    public string Username { get; private set; }
    public string Text { get; private set; }
    public DateTime CreatedAt { get; private set; }

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
    public string Username { get; private set; }
    public string EmojiType { get; private set; }

    public Emoji(string username, string emojiType)
    {
        Username = username;
        EmojiType = emojiType;
    }
}
