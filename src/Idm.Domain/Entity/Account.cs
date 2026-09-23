

namespace Idm.Domain.Entities;

public sealed class Account
{
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public AccountType AccountType { get; set; }
    public bool IsActive { get; set; }
    public int SystemId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

}

public enum AccountType
{

    Regular,
    Service,
    Privileged
}
