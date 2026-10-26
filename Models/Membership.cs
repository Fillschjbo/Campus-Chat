namespace CampusChat.Models;

public record Membership(int userId, int threadId)
{
    public DateTime LastReadUtc { get; set; }
};