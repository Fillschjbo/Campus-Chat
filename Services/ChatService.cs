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
    /// <summary>Raised after a profile has been updated.</summary>
    public event Action? ProfileChanged;
    /// <summary>Raised after a user joins or leaves a thread.</summary>
    public event Action? MembershipChanged;
    /// <summary>Raised after a post is created or deleted.</summary>
    public event Action? PostsChanged;
    /// <summary>Raised after a user's read marker for a thread moves.</summary>
    public event Action? ReadStateChanged;
    /// <summary>Raised after a direct message is sent.</summary>
    public event Action? MessagesChanged;
    

    /// <summary>
    /// Creates the service and fills it with seed profiles, threads, posts, memberships and a direct chat.
    /// </summary>
    public ChatService()
    {
        //Seed data
        _profiles.Add(new Profile("Felipe", _nextProfileId++, 1, "I love Backend", "#BADA55", Role.Student));
        _profiles.Add(new Profile("Craig", _nextProfileId++, 0, "I love Backend", "#67AD67", Role.Lecturer));
        _profiles.Add(new Profile("PH-ili-P", _nextProfileId++, 2, "I Hate Backend", "#123ABC", Role.Student));
        _profiles.Add(new Profile("DaAuditor67", _nextProfileId++, 0, "I know my rights!!", "#FEFEFE", Role.Auditor));

        _threads.Add(new ChatThread("Cafeteria rules", _nextThreadId++, DateTime.UtcNow.AddHours(-3), false, 4));
        _threads.Add(new ChatThread("Lecturers safespace", _nextThreadId++, DateTime.UtcNow.AddHours(-12), true, 2));
        _threads.Add(new ChatThread("Backend", _nextThreadId++, DateTime.UtcNow.AddHours(-5), false, 2));
        _threadCodes.Add(2, "1234");

        _posts.Add(new Post(_nextPostId++, 1, 1, "Is it appropriate to eat stinky cheese in the cafeteria?",
            "I really love eating stinky cheese and would like to do so in the cafeteria", DateTime.UtcNow.AddHours(-2),
            null));
        _posts.Add(new Post(_nextPostId++, 1, 2, null, "Please dont its really smelly.", DateTime.UtcNow.AddHours(-1),
            1));
        _posts.Add(new Post(_nextPostId++, 2, 2, "Stinky cheese trending among students",
            "It has come to my attention that our students have fallen in love with stinky cheeses. We need to take action!",
            DateTime.UtcNow.AddHours(-1), null));
        _posts.Add(new Post(_nextPostId++, 3, 3, "Please teach us PHP!",
            "I think php would be a valuable addition to the course", DateTime.UtcNow.AddHours(-4), null));

        _memberships.Add(new Membership(1, 1));
        _memberships.Add(new Membership(1, 3));
        _memberships.Add(new Membership(2, 1));
        _memberships.Add(new Membership(2, 2));
        _memberships.Add(new Membership(2, 3));
        _memberships.Add(new Membership(3, 3));

        _directChats.Add(new DirectChat(_nextDirectChatId++, 1, 2));
        _messages.Add(new Message(1, 2, _nextMessageId++, "Stop bringing cheese to class!",
            DateTime.UtcNow.AddHours(-2)));
        _messages.Add(new Message(1, 1, _nextMessageId++, "Its just Vieux-Boulogne, relax",
            DateTime.UtcNow.AddHours(-1)));
    }

    /// <summary>
    /// Gets all profiles.
    /// </summary>
    /// <returns>All profiles. Empty when there are none. Not null.</returns>
    public IReadOnlyList<Profile> GetAllProfiles()
    {
        lock (_lock)
        {
            return _profiles.ToList();
        }
    }

    /// <summary>Finds a profile by its user ID.</summary>
    /// <param name="profileId">The user ID to look up.</param>
    /// <returns>The profile, or <c>null</c> if no user has that ID.</returns>
    public Profile? GetProfileById(int profileId)
    {
        lock (_lock)
        {
            return _profiles.FirstOrDefault(p => p.UserId == profileId);
        }
    }

    /// <summary>Creates a new profile. New profiles always get the <see cref="Role.Student"/> role.</summary>
    /// <param name="displayName">Name shown to other users. Required.</param>
    /// <param name="year">Study year, 1 to 5.</param>
    /// <param name="note">Short profile note. Required.</param>
    /// <param name="avatarColorHex">Avatar colour as a hex string. Required.</param>
    /// <returns>The created profile with its new ID.</returns>
    /// <exception cref="ArgumentException">Thrown when any of the inputs are invalid.</exception>
    public Profile CreateProfile(string displayName, int year, string note, string avatarColorHex)
    {
        ValidateProfileInput(displayName, year, note, avatarColorHex, Role.Student);

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

    /// <summary>Updates the caller's own profile and raises <see cref="ProfileChanged"/>.</summary>
    /// <param name="callerId">The user editing their profile.</param>
    /// <param name="displayName">New display name. Required.</param>
    /// <param name="year">New study year. 0 is allowed for lecturers, otherwise 1 to 5.</param>
    /// <param name="note">New profile note. Required.</param>
    /// <param name="avatarColorHex">New avatar colour as a hex string. Required.</param>
    /// <exception cref="ArgumentException">Thrown when any of the inputs are invalid.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the caller has no profile.</exception>
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

    /// <summary>Checks profile fields and throws on the first invalid one.</summary>
    /// <param name="displayName">Display name to check.</param>
    /// <param name="year">Study year to check. Only checked for students.</param>
    /// <param name="note">Note to check.</param>
    /// <param name="avatarColorHex">Avatar colour to check.</param>
    /// <param name="role">The role the year should be validated against.</param>
    /// <exception cref="ArgumentException">Thrown when a field is invalid.</exception>
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

    /// <summary>Gets every thread the user is a member of.</summary>
    /// <param name="userId">The user whose memberships are used.</param>
    /// <returns>The joined threads. Empty if the user has joined none. Not null.</returns>
    public IReadOnlyList<ChatThread> GetChatThreadsJoinedBy(int userId)
    {
        lock (_lock)
        {
            var threadIds = _memberships.Where(m => m.UserId == userId).Select(m => m.ThreadId);
            return _threads.Where(t => threadIds.Contains(t.ThreadId)).ToList();
        }
    }

    /// <summary>
    /// The single access rule for threads. Public threads are readable by everyone, private threads only by
    /// members and auditors. Must be called while holding <c>_lock</c>.
    /// </summary>
    /// <param name="callerId">The signed in user, or <c>null</c> for a signed out visitor.</param>
    /// <param name="threadId">The thread to check.</param>
    /// <returns><c>true</c> if the caller may read the thread; <c>false</c> otherwise or if the thread does not exist.</returns>
    private bool CanRead(int? callerId, int threadId)
    {
        var thread = _threads.FirstOrDefault(thread => thread.ThreadId == threadId);

        if (thread == null)
        {
            return false;
        }

        if (!thread.IsPrivate)
        {
            return true;
        }

        if (callerId == null)
        {
            return false;
        }

        var profile = _profiles.FirstOrDefault(profile => profile.UserId == callerId.Value);

        if (profile?.Role == Role.Auditor)
        {
            return true;
        }

        return _memberships.Any(membership => membership.UserId == callerId.Value && membership.ThreadId == threadId);
    }

    /// <summary>Finds a thread by ID. Returns metadata only, so no access check is done.</summary>
    /// <param name="threadId">The thread ID to look up.</param>
    /// <returns>The thread, or <c>null</c> if it does not exist.</returns>
    public ChatThread? GetChatThreadById(int threadId)
    {
        lock (_lock)
        {
            return _threads.FirstOrDefault(thread => thread.ThreadId == threadId);
        }
    }

    /// <summary>Gets all posts and replies in a thread, including soft deleted posts.</summary>
    /// <param name="callerId">The signed in user, or <c>null</c> for a signed out visitor.</param>
    /// <param name="threadId">The thread to read.</param>
    /// <returns>The posts in the order they were created. Not null.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the thread does not exist.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when the caller may not read the thread.</exception>
    public IReadOnlyList<Post> GetPosts(int? callerId, int threadId)
    {
        lock (_lock)
        {
            if (!_threads.Any(thread => thread.ThreadId == threadId))
            {
                throw new KeyNotFoundException($"No thread with ID {threadId} exists.");
            }

            if (!CanRead(callerId, threadId))
            {
                throw new UnauthorizedAccessException("You do not have permission to read this private thread.");
            }

            return _posts.Where(post => post.ThreadId == threadId).ToList();
        }
    }
    
    /// <summary>Gets all threads, optionally filtered by name.</summary>
    /// <param name="query">Case-insensitive text the thread name must contain. Null or blank returns every thread.</param>
    /// <returns>The matching threads. Not null.</returns>
    public IReadOnlyList<ChatThread> GetAllThreads(string? query = null)
    {
        lock (_lock)
        {
            IEnumerable<ChatThread> result = _threads;

            if (!string.IsNullOrWhiteSpace(query))
            {
                result = result.Where(thread => thread.Name.Contains(query.Trim(), StringComparison.OrdinalIgnoreCase));
            }

            return result.ToList();
        }
    }

    /// <summary>Checks if a user has joined a thread.</summary>
    /// <param name="userId">The user to check.</param>
    /// <param name="threadId">The thread to check.</param>
    /// <returns><c>true</c> if a membership exists.</returns>
    public bool IsMember(int userId, int threadId)
    {
        lock (_lock)
        {
            return _memberships.Any(membership => membership.UserId == userId && membership.ThreadId == threadId);
        }
    }

    /// <summary>
    /// Adds the caller as a member of a thread and raises <see cref="MembershipChanged"/>.
    /// Joining a thread you are already in does nothing. New members start with everything marked as read.
    /// </summary>
    /// <param name="callerId">The user joining.</param>
    /// <param name="threadId">The thread to join.</param>
    /// <param name="code">The passcode for private threads. Ignored for public threads.</param>
    /// <exception cref="KeyNotFoundException">Thrown when the thread or user does not exist.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when the passcode for a private thread is wrong.</exception>
    public void JoinChatThread(int callerId, int threadId, string? code)
    {
        lock (_lock)
        {
            var thread = _threads.FirstOrDefault(thread => thread.ThreadId == threadId);

            if (thread == null)
            {
                {
                    throw new KeyNotFoundException($"No thread with ID {threadId} exists.");
                }

            }

            if (!_profiles.Any(profile => profile.UserId == callerId))
            {
                throw new KeyNotFoundException($"No user with ID {callerId} exists.");
            }

            if (_memberships.Any(membership => membership.UserId == callerId && membership.ThreadId == threadId))
            {
                return;
            }

            if (thread.IsPrivate)
            {
                if (!_threadCodes.TryGetValue(threadId, out var expectedCode) ||
                    !string.Equals(code, expectedCode, StringComparison.Ordinal))
                {
                    throw new UnauthorizedAccessException($"The private thread code you typed is wrong");
                }
            }
            _memberships.Add(new Membership(callerId, threadId)
            {
                LastReadUTC = DateTime.UtcNow
            });
        }
        MembershipChanged?.Invoke();
    }

    /// <summary>Removes the caller's membership and raises <see cref="MembershipChanged"/>. Does nothing if they were not a member.</summary>
    /// <param name="callerId">The user leaving.</param>
    /// <param name="threadId">The thread to leave.</param>
    /// <exception cref="KeyNotFoundException">Thrown when the thread or user does not exist.</exception>
    public void LeaveThread(int callerId, int threadId)
    {
        lock (_lock)
        {
            if (!_profiles.Any(profile => profile.UserId == callerId))
            {
                throw new KeyNotFoundException($"No user with ID {callerId} exists.");
            }

            if (!_threads.Any(thread => thread.ThreadId == threadId))
            {
                throw new KeyNotFoundException($"No thread with ID {threadId} exists.");
            }
            
            var membership = _memberships.FirstOrDefault(membership => membership.UserId == callerId && membership.ThreadId == threadId);

            if (membership == null)
            {
                return;
            }
            _memberships.Remove(membership);
        }
        MembershipChanged?.Invoke();
    }

    /// <summary>
    /// Creates a post or a reply and raises <see cref="PostsChanged"/>. The caller only needs read access,
    /// not membership. Auditors are read-only and cannot post.
    /// </summary>
    /// <param name="callerId">The author.</param>
    /// <param name="threadId">The thread to post in.</param>
    /// <param name="title">Optional title. Blank titles are stored as <c>null</c>.</param>
    /// <param name="body">The post text. Required.</param>
    /// <param name="parentPostId">The post being replied to, or <c>null</c> for a top level post.</param>
    /// <returns>The created post.</returns>
    /// <exception cref="ArgumentException">Thrown when the body is empty or the parent is in another thread.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the user, thread or parent post does not exist.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when the caller is an auditor or may not read the thread.</exception>
    public Post CreatePost(int callerId, int threadId, string? title, string body, int? parentPostId)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            throw new ArgumentException("Post body cannot be empty. Please write a message before posting.", nameof(body));
        }
        
        Post createdPost;

        lock (_lock)
        {
            var profile = _profiles.FirstOrDefault(profile => profile.UserId == callerId);

            if (profile == null)
            {
                throw new KeyNotFoundException($"No user with ID {callerId} exists.");
            }

            if (profile.Role == Role.Auditor)
            {
                throw new UnauthorizedAccessException("Auditors can read threads but cannot post in them.");
            }

            if (!_threads.Any(thread => thread.ThreadId == threadId))
            {
                throw new KeyNotFoundException($"No thread with ID {threadId} exists.");
            }

            if (!CanRead(callerId, threadId))
            {
                throw new UnauthorizedAccessException("You dont have access to this thread");
            }

            if (parentPostId.HasValue)
            {
                var parent = _posts.FirstOrDefault(post => post.PostId == parentPostId.Value);

                if (parent == null)
                {
                    throw new KeyNotFoundException($"Parent post {parentPostId.Value} does not exist.");
                }

                if (parent.ThreadId != threadId)
                {
                    throw new ArgumentException("A reply must belong to the same thread as its parent post.", nameof(parentPostId));
                }
            }
            var post = new Post(
                _nextPostId++,
                threadId,
                callerId,
                string.IsNullOrWhiteSpace(title) ? null : title.Trim(),
                body.Trim(),
                DateTime.UtcNow,
                parentPostId);

            _posts.Add(post);
            
            createdPost = post;
            
        }
        PostsChanged?.Invoke();
        return createdPost;
    }
    
    /// <summary>
    /// Deletes one of the caller's own posts and raises <see cref="PostsChanged"/>. A post with replies is
    /// soft deleted so the replies keep their parent; a post without replies is removed.
    /// </summary>
    /// <param name="callerId">The user deleting the post. Must be the author.</param>
    /// <param name="postId">The post to delete.</param>
    /// <exception cref="KeyNotFoundException">Thrown when the post does not exist.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when the caller cannot read the thread or is not the author.</exception>
    public void DeletePost(int callerId, int postId)
    {
        lock (_lock)
        {
            var post = _posts.FirstOrDefault(p => p.PostId == postId);

            if (post == null)
            {
                throw new KeyNotFoundException(
                    $"Post with ID {postId} does not exist.");
            }

            if (!CanRead(callerId, post.ThreadId))
            {
                throw new UnauthorizedAccessException(
                    "You do not have permission to delete this post.");
            }

            if (post.AuthorId != callerId)
            {
                throw new UnauthorizedAccessException(
                    "You can only delete your own posts.");
            }
            var hasReplies = _posts.Any(
                p => p.ParentPostId == postId);

            if (hasReplies)
            {
                post.IsDeleted = true;
            }
            else
            {
                _posts.Remove(post);
            }
        }

        PostsChanged?.Invoke();
    }

    /// <summary>
    /// Counts unread posts in each thread the user has joined. A post is unread if it was created after the
    /// user's read marker, was written by someone else and is not deleted. Replies count as posts.
    /// </summary>
    /// <param name="userId">The user to count for.</param>
    /// <returns>Thread ID to unread count, with an entry for every joined thread. Not null.</returns>
    public IReadOnlyDictionary<int, int> GetUnreadCounts(int userId)
    {
        lock (_lock)
        {
            var  memberships = _memberships.Where(memberships => memberships.UserId == userId).ToList();

            var unreadCounts = new Dictionary<int, int>();

            foreach (var membership in memberships)
            {
                var count = _posts.Count(post => post.ThreadId == membership.ThreadId && post.CreatedAtUTC > membership.LastReadUTC && post.AuthorId != userId && !post.IsDeleted);

                unreadCounts[membership.ThreadId] = count;
            }
            return unreadCounts;
        }
    }

    /// <summary>Moves the user's read marker for a thread to now and raises <see cref="ReadStateChanged"/>. Does nothing for non-members.</summary>
    /// <param name="userId">The user who read the thread.</param>
    /// <param name="threadId">The thread that was read.</param>
    public void MarkThreadRead(int userId, int threadId)
    {
        bool changed = false;

        lock (_lock)
        {
            var membership = _memberships.FirstOrDefault(memberships =>
                memberships.UserId == userId && memberships.ThreadId == threadId);

            if (membership != null)
            {
                membership.LastReadUTC = DateTime.UtcNow;
                changed = true;
            }
        }

        if (changed)
        {
            ReadStateChanged?.Invoke();
        }
    }
    
    /// <summary>
    /// The access rule for direct chats. Only the two participants and auditors may read. Must be called
    /// while holding <c>_lock</c>.
    /// </summary>
    /// <param name="callerId">The signed in user, or <c>null</c> for a signed out visitor.</param>
    /// <param name="chatId">The direct chat to check.</param>
    /// <returns><c>true</c> if the caller may read the chat.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the chat does not exist.</exception>
    private bool CanReadDirectChat(int? callerId, int chatId)
    {
        var chat = _directChats.FirstOrDefault(c => c.DirectChatId == chatId);

        if (chat == null)
            throw new KeyNotFoundException(
                $"No direct chat with ID {chatId} exists.");

        if (callerId == null)
            return false;

        var profile = _profiles.FirstOrDefault(
            p => p.UserId == callerId.Value);

        if (profile?.Role == Role.Auditor)
            return true;

        return chat.UserIdLowest == callerId.Value ||
               chat.UserIdHighest == callerId.Value;
    }
    
    /// <summary>Gets the messages in a direct chat, oldest first.</summary>
    /// <param name="callerId">The signed in user, or <c>null</c> for a signed out visitor.</param>
    /// <param name="chatId">The direct chat to read.</param>
    /// <returns>The messages. Not null.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the chat does not exist.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when the caller may not read the chat.</exception>
    public IReadOnlyList<Message> GetMessages(int? callerId, int chatId)
    {
        lock (_lock)
        {
            if (!CanReadDirectChat(callerId, chatId))
                throw new UnauthorizedAccessException(
                    "You are not allowed to read this direct chat.");

            return _messages.Where(message => message.ChatId == chatId).OrderBy(message => message.SentAtUTC).ToList();
        }
    }
    
    /// <summary>
    /// Gets the direct chats the caller takes part in, or every chat for auditors. Sorted by most recent
    /// message first; chats without messages come last.
    /// </summary>
    /// <param name="callerId">The signed in user.</param>
    /// <returns>The chats. Not null.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the user does not exist.</exception>
    public IReadOnlyList<DirectChat> GetDirectChatsFor(int callerId)
    {
        lock (_lock)
        {
            var profile = _profiles.FirstOrDefault(profile => profile.UserId == callerId);

            if (profile == null)
                throw new KeyNotFoundException($"No user with ID {callerId} exists.");

            var chats = profile.Role == Role.Auditor ? _directChats.ToList() : _directChats.Where(chat => chat.UserIdLowest == callerId || chat.UserIdHighest == callerId).ToList();

            return chats
                .OrderByDescending(chat => _messages.Where(m => m.ChatId == chat.DirectChatId).Select(m => (DateTime?)m.SentAtUTC).Max()).ToList();
        }
    }
    
    /// <summary>Returns the existing direct chat between two users, or creates it if there is none.</summary>
    /// <param name="callerId">The user starting the chat.</param>
    /// <param name="otherUserId">The other participant.</param>
    /// <returns>The direct chat between the two users.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when either user does not exist.</exception>
    /// <exception cref="ArgumentException">Thrown when both IDs are the same user.</exception>
    public DirectChat GetOrCreateDirectChat(int callerId, int otherUserId)
    {
        lock (_lock)
        {
            if (!_profiles.Any(p => p.UserId == callerId))
                throw new KeyNotFoundException($"No user with ID {callerId} exists.");

            if (!_profiles.Any(p => p.UserId == otherUserId))
                throw new KeyNotFoundException($"No user with ID {otherUserId} exists.");

            if (callerId == otherUserId)
                throw new ArgumentException("You cannot start a direct chat with yourself.", nameof(otherUserId));

            var existing = _directChats.FirstOrDefault(chat => chat.UserIdLowest == Math.Min(callerId, otherUserId) && chat.UserIdHighest == Math.Max(callerId, otherUserId));

            if (existing != null)
                return existing;

            var chat = new DirectChat(_nextDirectChatId++, callerId, otherUserId);

            _directChats.Add(chat);

            return chat;
        }
    }
    
    /// <summary>Sends a message in a direct chat and raises <see cref="MessagesChanged"/>. Auditors cannot send.</summary>
    /// <param name="callerId">The sender. Must be a participant.</param>
    /// <param name="chatId">The direct chat to send in.</param>
    /// <param name="text">The message text. Required.</param>
    /// <exception cref="ArgumentException">Thrown when the text is empty.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the chat does not exist.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when the caller is not a participant or is an auditor.</exception>
    public void SendMessage(int callerId, int chatId, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Message cannot be empty.", nameof(text));

        lock (_lock)
        {
            if (!CanReadDirectChat(callerId, chatId))
                throw new UnauthorizedAccessException("You are not allowed to send messages in this direct chat.");

            var profile = _profiles.FirstOrDefault(profile => profile.UserId == callerId);

            if (profile?.Role == Role.Auditor)
                throw new UnauthorizedAccessException("Auditors can read direct chats but cannot send messages.");

            var message = new Message(ChatId: chatId, SenderId: callerId, MessageId: _nextMessageId++, Text: text.Trim(), SentAtUTC: DateTime.UtcNow);

            _messages.Add(message);
        }

        MessagesChanged?.Invoke();
    }
    
    /// <summary>Finds a direct chat by ID. No access check is done.</summary>
    /// <param name="chatId">The chat ID to look up.</param>
    /// <returns>The chat, or <c>null</c> if it does not exist.</returns>
    public DirectChat? GetDirectChatById(int chatId)
    {
        lock (_lock)
        {
            return _directChats.FirstOrDefault(c => c.DirectChatId == chatId);
        }
    }
}