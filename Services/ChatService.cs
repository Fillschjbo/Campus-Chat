using CampusChat.Enums;
using CampusChat.Models;

namespace CampusChat.Services;

public class ChatService
{
    private readonly List<Profile> _profiles = [];
    private readonly List<ChatThread> _threads = [];
    private readonly List<Post> _posts = [];
    private readonly List<DirectChat> _directChats = [];
    private readonly List<Message> _messages = [];
    private readonly List<Membership> _memberships = [];
    private readonly Dictionary<int, string> _threadCodes = new();
    private readonly object _lock = new();
    private int _nextProfileId = 1;
    private int _nextThreadId = 1;
    private int _nextPostId = 1;
    private int _nextDirectChatId = 1;
    private int _nextMessageId = 1;
    public event Action? ProfileChanged;

    public ChatService()
    {
        //Seed data
        _profiles.Add(new Profile("Felipe", _nextProfileId++, 1, "I love Backend", "#BADA55", Role.Student));
        _profiles.Add(new Profile("Craig", _nextProfileId++, 0, "I love Backend", "#67AD67", Role.Lecturer));
        _profiles.Add(new Profile("PH-ili-P", _nextProfileId++, 2, "I Hate Backend", "#123ABC", Role.Student));
        _profiles.Add(new Profile("DaAuditor67", _nextProfileId++, 0, "I know my rights!!", "#FEFEFE", Role.Auditor));
        
        _threads.Add(new ChatThread("Cafeteria rules", _nextThreadId++, DateTime.UtcNow.AddHours(-3), false, 4 ));
        _threads.Add(new ChatThread("Lecturers safespace",  _nextThreadId++, DateTime.UtcNow.AddHours(-12), true, 2));
        _threads.Add(new ChatThread("Backend", _nextThreadId++, DateTime.UtcNow.AddHours(-5), false, 2));
        _threadCodes.Add(2,"1234");
        
        _posts.Add(new Post(_nextPostId++, 1, 1, "Is it appropriate to eat stinky cheese in the cafeteria?", "I really love eating stinky cheese and would like to do so in the cafeteria", DateTime.UtcNow.AddHours(-2), null));
        _posts.Add(new Post(_nextPostId++, 1, 2, null, "Please dont its really smelly.", DateTime.UtcNow.AddHours(-1), 1));
        _posts.Add(new Post(_nextPostId++, 2, 2, "Stinky cheese trending among students", "It has come to my attention that our students have fallen in love with stinky cheeses. We need to take action!", DateTime.UtcNow.AddHours(-1), null));
        _posts.Add(new Post(_nextPostId++, 3, 3, "Please teach us PHP!", "I think php would be a valuable addition to the course",  DateTime.UtcNow.AddHours(-4), null));
        
        _memberships.Add(new Membership(1, 1));
        _memberships.Add(new Membership(1, 3));
        _memberships.Add(new Membership(2, 1));
        _memberships.Add(new Membership(2, 2));
        _memberships.Add(new Membership(2, 3));
        _memberships.Add(new Membership(3, 3));
        
        _directChats.Add(new DirectChat(_nextDirectChatId++, 1, 2));
        _messages.Add(new Message(1, 2, _nextMessageId++, "Stop bringing cheese to class!", DateTime.UtcNow.AddHours(-2)));
        _messages.Add(new Message(1, 1, _nextMessageId++, "Its just Vieux-Boulogne, relax", DateTime.UtcNow.AddHours(-1)));
    }

    /// <summary>
    /// Gets all profiles
    /// </summary>
    /// <returns>A list of profiles or profiles when there are none. Not null</returns>
    public IReadOnlyList<Profile> GetAllProfiles()
    {
        lock (_lock)
        {
            return _profiles.ToList();
        }
    }

    public Profile? GetProfileById(int profileId)
    {
        lock (_lock)
        {
           return _profiles.FirstOrDefault(p => p.UserId == profileId);
        }
    }

    public Profile CreateProfile(string displayName, int year, string note, string avatarColorHex)
    {
        ValidateProfileInput(displayName, year, note, avatarColorHex,  Role.Student);

        lock (_lock)
        {
            var profile = new Profile(
                displayName,
                _nextProfileId++,
                year,
                note,
                avatarColorHex,
                Role.Student);

            _profiles.Add(profile);

            return profile;
        }
    }
    
    public void UpdateProfile(
        int callerId,
        string displayName,
        int year,
        string note,
        string avatarColorHex)
    {
        ValidateProfileInput(displayName, year, note, avatarColorHex, year == 0 ? Role.Lecturer : Role.Student);

        lock (_lock)
        {
            var profile = _profiles.FirstOrDefault(p => p.UserId == callerId);

            if (profile == null)
            {
                throw new InvalidOperationException(
                    $"No profile exists for user ID {callerId}.");
            }

            profile.DisplayName = displayName;
            profile.Year = year;
            profile.Note = note;
            profile.AvatarColorHex = avatarColorHex;
        }

        ProfileChanged?.Invoke();
    }
    
    private static void ValidateProfileInput(
        string displayName,
        int year,
        string note,
        string avatarColorHex,
        Role role)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Display name is required.", nameof(displayName));

        if (role == Role.Student && (year < 1 || year > 5))
            throw new ArgumentException("Year must be between 1 and 5.", nameof(year));

        if (string.IsNullOrWhiteSpace(note))
            throw new ArgumentException("Note is required.", nameof(note));

        if (string.IsNullOrWhiteSpace(avatarColorHex))
            throw new ArgumentException("Avatar color is required.", nameof(avatarColorHex));
    }
    
    public IReadOnlyList<ChatThread> GetChatThreadsJoinedBy(int userId)
    {
        lock (_lock)
        {
            var threadIds = _memberships.Where(m => m.UserId == userId).Select(m => m.ThreadId);
            return _threads.Where(t => threadIds.Contains(t.ThreadId)).ToList();
        }
    }
}