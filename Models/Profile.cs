using System.ComponentModel;
using CampusChat.Enums;

namespace CampusChat.Models;

public class Profile
{
    public string DisplayName { get; set; }
    public int UserId { get; init; }
    public int Year { get; set; }
    public string Note { get; set; }
    public string AvatarColorHex { get; set; }
    public Role Role { get; set; }
    
    public Profile (string displayName, int userId, int year, string note, string avatarColorHex, Role role)
    {
        DisplayName = displayName;
        UserId = userId;
        Year = year;
        AvatarColorHex = avatarColorHex;
        Role = role;
    }
}

