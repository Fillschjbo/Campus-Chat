namespace CampusChat.Models;

public class Post
{
    public int PostId { get; }
    public int ThreadId { get; }
    public int AuthorId { get; }
    public string? Title { get; }
    public string Body { get; }
    public DateTime CreatedAtUTC { get; }
    public int? ParentPostId { get; }
    public bool IsDeleted { get; set; } = false;
    
    public Post(int postId, int threadId, int authorId, string? title, string body, DateTime createdAtUTC, int? parentPostId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(body);
        PostId = postId;
        ThreadId = threadId;
        AuthorId = authorId;
        Title = title;
        Body = body;
        CreatedAtUTC = createdAtUTC;
        ParentPostId = parentPostId;
    }
}