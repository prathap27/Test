namespace LeaveManagementApi.Authorization;

public class AuthSettings
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SigningKey { get; set; } = string.Empty;
    public int TokenLifetimeMinutes { get; set; } = 60;
    public List<AuthUser> Users { get; set; } = new();
    public List<string> ApprovalLevels { get; set; } = new();
}

public class AuthUser
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
