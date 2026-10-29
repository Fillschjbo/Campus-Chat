namespace CampusChat.Models;

public record Membership(int UserId, int ThreadId)
{
    public DateTime LastReadUTC { get; set; } = DateTime.MinValue;
}