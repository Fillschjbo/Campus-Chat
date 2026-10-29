# AI usage

## Setup (stage 0)

- I explained the Blazor Web App template, global interactivity, and why prerendering is off.

## Models (stage 1)

- I suggested `Membership` as its own record instead of lists on the user and the thread.
- I suggested keeping the private thread code in the service (`Dictionary<int,string>`), never in the model or the page.
- I suggested replies as `int? ParentPostId` on `Post` instead of their own class.

**Bugs I found:**

- The `Message` properties were never set.
- `List<Thread>` was the wrong type (it pointed to `System.Threading`).

## Services (stage 1–2)

- I explained DI lifetimes (singleton vs scoped), `lock`, and returning copies.

**Bugs I found:**

- The static seed array (it broke §8.1 and CS0236).
- Statements in the class body.
- Off-by-one IDs in the seed.
- `GetAllProfiles` returning the real list outside the lock.

**`CurrentUserService`:** I pointed out the missing `Success` check, a catch that only re-threw, and `IsLoaded` being set only on success. I suggested `finally`, and renaming the event to `UserChanged`.

## Sign-in and nav (stage 2)

- I explained events (`+=`/`-=`, `Dispose`, `InvokeAsync(StateHasChanged)`) and how `OnInitializedAsync` works in `MainLayout`.
- I found the infinite loop (`LoadUserAsync` in the handler) and `async void`, and suggested the `LoadUser` helper pattern.

## Profiles (stage 3)

- I reviewed `GetProfileById`, `CreateProfile` (I pointed out that `Role` must not be a parameter, for security) and `GetChatThreadsJoinedBy`.
- I diagnosed that Rider's rename had changed `Profile` to `ProfilePage` in the sign-in page.

**Bugs I found:**

- The nav subscribing again on every load.
- The year rule stopping lecturers and the auditor.
- The sign-up crash.
- The role guess from `year`.

## Threads and posts (stage 4)

- I suggested one access rule (`CanRead`) with early returns, called inside the lock.
- I gave you the snippet `private Task Delete() => OnDelete.InvokeAsync(Post.PostId);`.

**Bugs I found:**

- `CreatePost` checked the title instead of the body.
- The cascade delete removed other people's replies.

## Unread counts (stage 5)

- I suggested `GetUnreadCounts` as a dictionary.

**Bugs I found:**

- No `lock` in `GetUnreadCounts`.
- The nav missing event subscriptions.

## Direct chats (stage 6)

- I suggested starting a chat from a button on the profile page.
