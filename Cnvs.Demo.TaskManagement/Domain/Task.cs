namespace Cnvs.Demo.TaskManagement.Domain;

public class Task
{
    public Task(Guid taskId, string taskDescription, TaskState taskState = TaskState.Waiting)
    {
        Id = taskId;
        Description = taskDescription ?? throw new ArgumentNullException(nameof(taskDescription));
        State = taskState;
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
    internal List<User> AssignedUsersHistory { get; set; } = new();
    public int TransferCount { get; set; }
}

public class NullTask : Task
{
    public static NullTask Instance { get; } = new NullTask();

    private NullTask() : base(Guid.Empty, "Null Task")
    {
    }
}