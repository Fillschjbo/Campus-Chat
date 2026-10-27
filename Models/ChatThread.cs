using CampusChat.Enums;

namespace CampusChat.Models;

public class ChatThread
{
    public string Name { get;  }
    public int ThreadId { get; }
    public DateTime CreatedAtUTC { get; }
    public bool IsPrivate { get; }
    public int OwnerId { get; }

    public ChatThread(string name, int threadId, DateTime createdAtUTC, bool isPrivate, int ownerId)
    {
        Name = name;
        ThreadId = threadId;
        CreatedAtUTC = createdAtUTC;
        IsPrivate = isPrivate;
        OwnerId = ownerId;
    }
}