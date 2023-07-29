namespace Cnvs.Demo.TaskManagement.Domain;

public class Task
{
    private protected Task(Guid taskId, string taskDescription)
    {
        Id = taskId;
        Description = taskDescription ?? throw new ArgumentNullException(nameof(taskDescription));
    }

    private Task(string taskDescription)
    {
        Id = Guid.NewGuid();
        Description = taskDescription ?? throw new ArgumentNullException(nameof(taskDescription));
    }

    public static Task NewTask(string taskDescription)
    {
        return new Task(taskDescription) { State = TaskState.Waiting };
    }

    /// <summary>
    /// Id is a unique identifier for the task.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Description is a short description of the task. Is not unique by requirement.
    /// </summary>
    public string Description { get; set; }
    
    public TaskState State { get; set; }
    public User? AssignedUser { get; set; }
    public List<User> AssignedUsersHistory { get; set; } = new();
    public int TransferCount { get; set; }
}