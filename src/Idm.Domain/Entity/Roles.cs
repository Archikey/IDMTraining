namespace Idm.Domain.Entities;


public sealed class Roles
{

    public int Id { get; set; }
    public string Description {get;set;} = string.Empty;
    public string NameRole { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int SystemId { get; set; }
}