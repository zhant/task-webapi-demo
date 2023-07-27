namespace Cnvs.Demo.TaskManagement.Dto;

public class Task
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Description { get; set; } = string.Empty;
    public TaskState State { get; set; } = TaskState.Waiting;
}

