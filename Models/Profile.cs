using CampusChat.Enums;

namespace CampusChat.Models;

public class Profile
{
    public string DisplayName { get; set; }
    public int UserId { get; }
    public int Year { get; set; }
    public string Note { get; set; }
    public string AvatarColorHex { get; set; }
    public Role Role { get; set; }
    
    /// <summary>Creates a profile.</summary>
    /// <param name="displayName">Name shown to other users.</param>
    /// <param name="userId">Unique user ID.</param>
    /// <param name="year">Study year. 0 for staff.</param>
    /// <param name="note">Short profile note.</param>
    /// <param name="avatarColorHex">Avatar colour as a hex string.</param>
    /// <param name="role">The user's role.</param>
    public Profile (string displayName, int userId, int year, string note, string avatarColorHex, Role role)
    {
        DisplayName = displayName;
        UserId = userId;
        Year = year;
        Note = note;
        AvatarColorHex = avatarColorHex;
        Role = role;
    }
}

