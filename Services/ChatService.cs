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
    
}