using CampusChat.Enums;

namespace CampusChat.Models;

public class ChatThread
{
    public string Name {get; set;}
    public int ThreadId {get; init;}
    public DateTime CreatedAt {get;set;}
    public bool IsPrivate {get;set;}
    public int Owner {get;set;}

    public ChatThread(string name, int threadId, DateTime createdAt, bool isPrivate, int owner)
    {
        Name = name;
        ThreadId = threadId;
        CreatedAt = createdAt;
        IsPrivate = isPrivate;
        Owner = owner;
    }
}