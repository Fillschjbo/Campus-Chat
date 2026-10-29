namespace CampusChat.Models;

public record Message (int ChatId, int SenderId, int MessageId, string Text, DateTime SentAtUTC);