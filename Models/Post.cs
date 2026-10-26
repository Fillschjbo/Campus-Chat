namespace CampusChat.Models;

public class Post
{
    public int Id { get; }
    public int ThreadId { get; }
    public int AuthorId { get; }
    public string? Title { get; }
    public string Body { get; set; }
    public DateTime CreatedAtUTC { get; }
    public int? ParentPostId { get; init; }
    
    public Post(int id, int threadId, int authorId, string title, string body, DateTime createdAtUTC, int? parentPostId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(body);
        Id = id;
        ThreadId = threadId;
        AuthorId = authorId;
        Title = title;
        Body = body;
        CreatedAtUTC = createdAtUTC;
        ParentPostId = parentPostId;
    }
}