namespace CampusChat.Models;

public class DirectChat
{
    public int DirectChatId { get; }
    public int UserIdLowest { get; }
    public int UserIdHighest { get; }
    
    public DirectChat(int directChatId, int userIdLowest, int userIdHighest)
    {
        if (userIdLowest > userIdHighest)
            (userIdLowest, userIdHighest) = (userIdHighest, userIdLowest);
        
        DirectChatId = directChatId;
        UserIdLowest = userIdLowest;
        UserIdHighest = userIdHighest;
    }
}