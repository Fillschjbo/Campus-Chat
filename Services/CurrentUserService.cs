using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace CampusChat.Services;

public class CurrentUserService (ProtectedSessionStorage storage)
{
    public int? CurrentUserId { get; private set; }
    public bool isLoaded  { get; private set; }

    public async Task SignInAsync(int userId)
    {
        CurrentUserId = userId;
        await storage.SetAsync("StoredUser", userId);
        UserChanged?.Invoke();
    }

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

    public async Task SignOutAsync()
    {
        await storage.DeleteAsync("StoredUser");
        CurrentUserId = null;
        UserChanged?.Invoke();
    }

    public event Action? UserChanged;
}