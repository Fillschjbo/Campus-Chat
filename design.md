## Project in the root folder:
I have put the project int the root folder since the assignment asks that build commands can be run inside the clone of the project. dotnet run fails in a folder that only holds a .sln.

## Global interactivity:
I set the project up with global interactivity since every page needs a live connection anyway, and forgetting to add @rendermode interactiveServer on a page can cause a massive headache and unnecessary troubleshooting

## Profile class:
The profile class stores displayname, id, note, avatarcolor, year and role. Instead of having a list of threads the user has joined i decided to have a membership record that stores the userid and later on the threadid. The role enum has student, lecturer and auditor as roles.

## Chat Thread class:
the chat thread class takes id, name, created at date time, owner and a boolean to see if the thread is private. The passcode is stored in a dictionary on the server where and is linked with its thread id. a page only sends a code and recives a correct or wrong answer from the server.

## Service lifetimes:
ChatService is registered as a singleton. All the data lives in lists inside it, so there has to be exactly one instance that every user and every browser tab shares, otherwise two users would never see each other's posts. Because many circuits call it at the same time, every read and write goes through one shared lock, and every method returns a copy of the list (ToList) so a page never iterates a list another thread is changing.

CurrentUserService is registered as scoped. In Blazor Server a scope is one circuit, which is one browser tab, so each tab has its own signed in user. If it was a singleton everyone would be signed in as the last person who clicked sign in.

## Session storage:
The signed in user ID is saved with ProtectedSessionStorage under the key "StoredUser". The value is encrypted with ASP.NET Data Protection so a user can't edit it in the dev tools to become someone else. Session storage is per tab, so it survives a reload but is gone when the tab is closed, and two tabs can be signed in as different users which is handy for testing.

Reading it needs JS interop, which is not available during prerendering, so prerendering is turned off on Routes in App.razor. MainLayout calls LoadUserAsync in OnInitializedAsync and shows "Loading..." until isLoaded is true, so no page renders as signed out for a split second before the user is restored. If reading fails the error is logged and the user is treated as signed out.

## The access rule lives in the service:
Who can read a thread is decided in one place, the private CanRead method in ChatService:
- the thread has to exist
- public threads can be read by everyone, also signed out visitors
- private threads can be read by members and auditors only

GetPosts, CreatePost and DeletePost all call CanRead and throw UnauthorizedAccessException if it fails. The pages catch the exception and show the join form instead of the posts. Hiding a button in the UI is just for looks; even if a page forgot to check, the service would still refuse. Direct chats have the same setup with CanReadDirectChat: only the two participants and auditors.

Thread metadata (GetChatThreadById, GetAllThreads) is not checked, so the name of a private thread is visible in the list and search. Only the posts are protected.

## Soft delete:
Users can only delete their own posts. If the post has replies it is soft deleted: IsDeleted is set to true and PostView shows "[deleted]" instead of the author and text. This keeps the replies attached to their parent so the conversation still makes sense. If the post has no replies it is removed from the list, since there is nothing to keep it for.

Soft deleted posts are not counted as unread. A known gap: if all replies under a soft deleted post are deleted later, the "[deleted]" placeholder stays.

## Posting without joining:
CreatePost only requires that the caller can read the thread, not that they are a member (auditors are the exception, they can't post at all). That means you can post in any public thread without joining it, and in a private thread you have to join with the code first (since that is the only way to get read access). Joining is about following a thread: it puts the thread under "My threads", on your profile and gives you unread counts for it.

## Sort order:
- Posts: top level posts are shown oldest first, in the order they were created. GetPosts doesn't sort since posts are appended to the list as they are created and IDs only go up.
- Replies: shown under their parent, oldest first (sorted by CreatedAtUTC in PostView), so a reply chain reads top to bottom like a conversation.
- Direct messages: oldest first, sorted by SentAtUTC in GetMessages, like every other chat app.
- Direct chat list: the chat with the most recent message first. Chats that have no messages yet end up last.
- Thread lists: in the order the threads were created.

## Unread and the §6 count decision:
Each Membership has a LastReadUTC. When you open a thread you are a member of, MarkThreadRead moves it to now. When you join a thread LastReadUTC is set to now too, so joining an old thread does not suddenly give you a big unread number for posts written before you joined.

GetUnreadCounts counts, for every joined thread, the posts where:
- the post was created after LastReadUTC
- the post was not written by you, your own posts are never unread
- the post is not soft deleted

Replies count the same as top level posts, since a reply is new content you haven't seen. Threads you haven't joined have no unread count at all, because you don't follow them and there is no read marker to compare against. The thread list shows the count per thread and the nav menu shows the total.

## Events and InvokeAsync:
ChatService exposes plain C# events: ProfileChanged, MembershipChanged, PostsChanged, ReadStateChanged and MessagesChanged. CurrentUserService has UserChanged. Components subscribe in OnInitialized and unsubscribe in Dispose (they all implement IDisposable). Unsubscribing matters because the singleton would otherwise keep a reference to every page ever opened, and call StateHasChanged on dead components.

The events are raised after the lock is released, so a subscriber that calls back into the service doesn't run while the lock is held.

An event runs on the thread of the user who caused it, not the thread of the component that listens. Blazor only allows a component to render on its own circuit's dispatcher, so every handler wraps the reload in InvokeAsync(() => { Load(); StateHasChanged(); }). Calling StateHasChanged directly from another user's thread throws "The current thread is not associated with the Dispatcher".

## The read-state event against the loop:
SingleThread.Load calls MarkThreadRead whenever a member opens the thread, and MarkThreadRead raises ReadStateChanged. If SingleThread listened to ReadStateChanged it would end up in a loop: Load -> MarkThreadRead -> ReadStateChanged -> Load -> and so on forever.

So SingleThread deliberately does not subscribe to ReadStateChanged. It only listens to PostsChanged, MembershipChanged and UserChanged. The components that do listen to ReadStateChanged (NavMenu and Threads) only read the unread counts and never mark anything as read, so the chain stops there. When a new post arrives while you have the thread open, PostsChanged reloads the page, which marks it read again, which updates the badge in the nav menu.

## The auditor is read-only:
The auditor role is for reading. An auditor can read every private thread without joining and can read every direct chat, including the ones they are not part of (GetDirectChatsFor returns all chats for an auditor). The auditor can't write anything. In direct chats SendMessage throws for auditors and SingleChat hides the message box. In threads CreatePost throws for auditors, the same way SendMessage does, and SingleThread shows "Viewing as auditor, read-only." instead of the post form, with no Reply button on posts. Like every other rule, the check is in the service, so hiding the UI is just for looks. On private threads the join button is hidden for an auditor who isn't a member, since they already have read access and joining would only give them unread counts.

## Routes:
| Route | Page | What it does |
|---|---|---|
| `/` | Threads | Search all threads, list of my threads with unread counts |
| `/threads/{Id:int}` | SingleThread | Read a thread, join/leave, post, reply and delete |
| `/chats` | Chats | List of my direct chats with the last message |
| `/chats/{ChatId:int}` | SingleChat | Read and send direct messages |
| `/profile/{Id:int}` | ProfilePage | Show a profile and its threads, start a direct chat |
| `/profile/edit` | EditProfile | Edit my own profile |
| `/signin` | SignIn | Pick a profile to sign in as |
| `/signup` | SignUp | Create a new profile |
| `/Error` | Error | Error page in production |
| `/not-found` | NotFound | Shown for unknown routes |

Route IDs use the `:int` constraint so `/threads/abc` goes to the not found page instead of crashing the page.
