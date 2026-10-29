# Safety

All data is kept in memory inside `ChatService`. Nothing is written to a database or a file, so everything is gone when the server restarts. The answers below describe what the product does now, and where a rule isn't built yet, what it should do.

## 1. Name every piece of personal data that the product stores.

- **Profile:** display name, user ID, study year, profile note, avatar colour and role (student, lecturer or auditor).
- **Memberships:** which threads each user has joined, and when they last read each thread (`LastReadUTC`). Together that shows what a user follows and when they were active.
- **Posts:** author, title, body, time posted, which post it replies to, and whether it was deleted. A soft deleted post keeps its author and text in memory; only the page hides them.
- **Threads:** who owns each thread.
- **Direct chats:** who talks to whom.
- **Direct messages:** sender, text and time sent.
- **In the browser:** the signed in user ID, encrypted in protected session storage for that tab.

There are no passwords, emails, phone numbers or real names (the display name is whatever the user types).

## 2. State how long the product keeps a direct chat. Justify the number.

**Now:** until the server restarts, because it is only kept in memory. There is no way to delete a chat or a message.

**Rule if the data is saved:** 12 months after the last message, then the chat and its messages are deleted. A year covers a whole college year, so a conversation about a course or group project is still there for the rest of that year, and the auditor has time to look into a complaint. Keeping it longer than that would only store private conversations no one needs any more. Each new message resets the 12 months, so an active chat is not deleted in the middle of a conversation.

## 3. A student leaves the college. State what happens to their posts. State what happens to their profile.

**Now:** nothing happens. There is no way to delete a profile, so both the profile and the posts stay until the server restarts.

**Rule:**

- **Profile:** deleted, together with their memberships, so they no longer show up anywhere and can't sign in.
- **Posts:** kept, so threads and the replies to them still make sense, but no longer linked to a person. `PostView` already shows "Unknown user" when the author's profile doesn't exist. Their direct chats follow the 12 month rule from question 2.

## 4. A user posts abuse in a thread. Name who removes the post. Name who decides.

**Now:** only the author can delete a post (`DeletePost` checks `AuthorId`). There is no moderation, so nobody else can remove abuse.

**Rule:**

- **Removes it:** a lecturer, using a moderator delete that soft deletes the post so the replies stay. Students can't remove other people's posts, and the auditor is read-only so they can't either.
- **Decides:** the college, following its code of conduct. The lecturer removes the post so it stops doing harm straight away; what happens to the user after that is for the college to decide, not the product.

## 5. A direct chat is private. State what stops another user from reading it. The audit role reads it anyway. State who holds that role. State whether the product tells the two people.

- **What stops other users:** `CanReadDirectChat` in `ChatService`. `GetMessages` checks it on every read and throws `UnauthorizedAccessException` unless the caller is one of the two people in the chat or an auditor. The check is in the service, not the page, so typing another chat's address (`/chats/{id}`) only shows "You don't have access to this conversation."
- **Who holds the audit role:** a person the college chooses. In the seed data that is `DaAuditor67`. The role can't be picked in the app: sign up always creates a student, and editing a profile can't change the role, so a user can't make themselves an auditor.
- **Does the product tell the two people:** yes, but only in general. Every direct chat shows "Messages in this chat can be read by the college's auditor." The product does **not** tell them when the auditor actually opens their chat.

## 6. Name one thing that the product must never log.

The text of direct messages. Other things that must never be logged are private thread codes and the session storage value.

The product doesn't log any of these today. The only logging is in `CurrentUserService.LoadUserAsync`, which writes the exception if session storage can't be read, not the stored user ID.

## 7. A private thread has a code. State where the product keeps that code.

In a private `Dictionary<int, string> _threadCodes` inside `ChatService`, on the server, keyed by thread ID. It is not in the `ChatThread` model and never goes to a page. The page only sends the code the user typed to `JoinChatThread` and gets back either a membership or a "wrong code" error. The code is stored in plain text, because it is only in memory. If the data were saved to disk, it should be hashed.
