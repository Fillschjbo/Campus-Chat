using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace CampusChat.Services;

/// <summary>
/// Keeps track of which user is signed in for one circuit (browser tab). The user ID is stored in
/// protected session storage so it survives a page reload.
/// </summary>
/// <param name="storage">Encrypted per-tab browser session storage.</param>
public class CurrentUserService (ProtectedSessionStorage storage)
{
    /// <summary>The signed in user's ID, or <c>null</c> when signed out.</summary>
    public int? CurrentUserId { get; private set; }
    /// <summary><c>true</c> once <see cref="LoadUserAsync"/> has finished, even if it failed.</summary>
    public bool isLoaded  { get; private set; }

    /// <summary>Signs in as the given user, saves the ID to session storage and raises <see cref="UserChanged"/>.</summary>
    /// <param name="userId">The user to sign in as.</param>
    public async Task SignInAsync(int userId)
    {
        CurrentUserId = userId;
        await storage.SetAsync("StoredUser", userId);
        UserChanged?.Invoke();
    }

    /// <summary>
    /// Restores the signed in user from session storage and raises <see cref="UserChanged"/>. Errors are
    /// logged and treated as signed out. Needs JS interop, so it cannot run during prerendering.
    /// </summary>
    public async Task LoadUserAsync()
    {
        ProtectedBrowserStorageResult<int> stored;
        try
        {
            stored = await storage.GetAsync<int>("StoredUser");
            if (stored.Success)
            {
                CurrentUserId = stored.Value;
            }
            else
            {
                CurrentUserId = null;
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            isLoaded = true;
            UserChanged?.Invoke();
        }
    }

    /// <summary>Signs out, clears session storage and raises <see cref="UserChanged"/>.</summary>
    public async Task SignOutAsync()
    {
        await storage.DeleteAsync("StoredUser");
        CurrentUserId = null;
        UserChanged?.Invoke();
    }

    /// <summary>Raised when the user signs in, signs out or is restored from storage.</summary>
    public event Action? UserChanged;
}