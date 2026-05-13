namespace PatientManagement.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string Role { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    private User() { }

    public User(string username, string passwordHash, string role = "Doctor")
    {
        Username = username?.Trim().ToLowerInvariant() ?? throw new ArgumentNullException(nameof(username));
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        Role = role?.Trim() ?? throw new ArgumentNullException(nameof(role));
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdatePassword(string passwordHash)
    {
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
    }
}
