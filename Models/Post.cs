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
    
    /// <summary>Creates a post or a reply.</summary>
    /// <param name="postId">Unique post ID.</param>
    /// <param name="threadId">The thread the post belongs to.</param>
    /// <param name="authorId">The user who wrote it.</param>
    /// <param name="title">Optional title.</param>
    /// <param name="body">The post text. Required.</param>
    /// <param name="createdAtUTC">When it was posted, in UTC.</param>
    /// <param name="parentPostId">The post this replies to, or <c>null</c> for a top level post.</param>
    /// <exception cref="ArgumentException">Thrown when the body is null or blank.</exception>
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