# Campus Chat

A Blazor Server chat app for a campus. Users can join public and private threads, write posts and replies, see how many posts they haven't read and send direct messages to each other. All data is kept in memory, so everything resets when the app restarts.

## How to run

You need the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
git clone https://github.com/Fillschjbo/Campus-Chat.git
cd Campus-Chat
dotnet run
```

Then open http://localhost:5128 (or https://localhost:7087 with `dotnet run --launch-profile https`).

The project is in the root of the repository, so `dotnet build` and `dotnet run` work straight from the clone.

## Signing in

There are no passwords. Go to **Sign in** and pick a profile, or create a new one under **Sign up** (new profiles are always students). The signed in user is stored per browser tab, so you can open two tabs as two different users to see live updates.

Seed users:

| User | Role | Notes |
|---|---|---|
| Felipe | Student | Member of Cafeteria rules and Backend, has a direct chat with Craig |
| Craig | Lecturer | Member of every thread |
| PH-ili-P | Student | Member of Backend |
| DaAuditor67 | Auditor | Can read every private thread and every direct chat, but can't post or send messages |

Seed threads: **Cafeteria rules** (public), **Backend** (public) and **Lecturers safespace** (private, code `1234`).

## Features

- Search and browse threads, join and leave them
- Private threads that need a code to join and read
- Posts with optional titles, nested replies and deleting your own posts
- Unread counts per thread and in total in the nav menu
- Profiles you can view and edit
- Direct messages between two users
- A read-only auditor role that can read everything without being a member
- Live updates across tabs and users without reloading

## Project structure

| Folder | Contents |
|---|---|
| `Components/Pages` | One Razor page per route |
| `Components/Posts` | `PostView`, which renders a post and its replies recursively |
| `Components/Layout` | Main layout and nav menu |
| `Models` | `Profile`, `ChatThread`, `Post`, `Membership`, `DirectChat`, `Message` |
| `Services` | `ChatService` (all data and rules, singleton) and `CurrentUserService` (signed in user, scoped) |
| `Enums` | `Role` |

The design decisions are explained in [design.md](design.md).

## What does not work yet

- Data is not saved anywhere; restarting the app resets it to the seed data.
- There is no real authentication. Anyone can sign in as any profile.
- Threads can't be created from the UI, only the seeded ones exist.
- Direct messages have no unread count.
- When every reply under a deleted post is deleted, the `[deleted]` placeholder stays.
