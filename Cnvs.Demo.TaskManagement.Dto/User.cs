namespace Cnvs.Demo.TaskManagement.Dto;

public class User
{
    public string? Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
