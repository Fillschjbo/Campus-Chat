using CampusChat.Enums;

namespace CampusChat.Models;

public class ChatThread
{
    public string Name { get;  }
    public int ThreadId { get; }
    public DateTime CreatedAtUTC { get; }
    public bool IsPrivate { get; }
    public int OwnerId { get; }

    /// <summary>Creates a thread. The passcode for private threads is kept in <c>ChatService</c>, not here.</summary>
    /// <param name="name">Thread name.</param>
    /// <param name="threadId">Unique thread ID.</param>
    /// <param name="createdAtUTC">When the thread was created, in UTC.</param>
    /// <param name="isPrivate">Whether a passcode is needed to join and read.</param>
    /// <param name="ownerId">The user who created the thread.</param>
    public ChatThread(string name, int threadId, DateTime createdAtUTC, bool isPrivate, int ownerId)
    {
        Name = name;
        ThreadId = threadId;
        CreatedAtUTC = createdAtUTC;
        IsPrivate = isPrivate;
        OwnerId = ownerId;
    }
}