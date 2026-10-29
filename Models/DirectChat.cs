namespace CampusChat.Models;

public class DirectChat
{
    public int DirectChatId { get; }
    public int UserIdLowest { get; }
    public int UserIdHighest { get; }
    
    /// <summary>Creates a direct chat between two users. The IDs are swapped if needed so the lowest is always first.</summary>
    /// <param name="directChatId">Unique chat ID.</param>
    /// <param name="userIdLowest">One participant.</param>
    /// <param name="userIdHighest">The other participant.</param>
    public DirectChat(int directChatId, int userIdLowest, int userIdHighest)
    {
        if (userIdLowest > userIdHighest)
            (userIdLowest, userIdHighest) = (userIdHighest, userIdLowest);
        
        DirectChatId = directChatId;
        UserIdLowest = userIdLowest;
        UserIdHighest = userIdHighest;
    }
}