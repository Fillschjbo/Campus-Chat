namespace CampusChat.Services;

public class CurrentUserService
{
    public int? CurrentUserId { get; private set; }

    public void SignIn(int userId)
    {
        CurrentUserId = userId;
    }
}