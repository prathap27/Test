using Microsoft.Extensions.Options;

namespace LeaveManagementApi.Authorization;

public interface IUserStore
{
    AuthUser? ValidateUser(string username, string password);
    AuthUser? GetById(Guid id);
}

public class UserStore : IUserStore
{
    private readonly List<AuthUser> _users;

    public UserStore(IOptions<AuthSettings> settings)
    {
        _users = settings.Value.Users;
    }

    public AuthUser? ValidateUser(string username, string password)
    {
        return _users.FirstOrDefault(user =>
            user.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
            && user.Password == password);
    }

    public AuthUser? GetById(Guid id)
    {
        return _users.FirstOrDefault(user => user.Id == id);
    }
}
