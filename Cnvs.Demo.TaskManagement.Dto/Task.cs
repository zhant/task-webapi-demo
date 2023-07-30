namespace Cnvs.Demo.TaskManagement.Dto;

public class Task
{
    public Guid Id { get; set; }
    public string Description { get; set; } = null!;
    public TaskState State { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? SuspendedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

