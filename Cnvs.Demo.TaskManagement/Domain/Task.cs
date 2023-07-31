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
        return new Task(taskDescription) { State = TaskState.Waiting, CreatedAt = DateTime.UtcNow };
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
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? SuspendedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public User AssignedUser { get; set; } = NullUser.Instance;
    public List<User> AssignedUsersHistory { get; set; } = new();
    public int TransferCount { get; set; }
    public bool IsDeleted { get; set; }

    public void Complete()
    {
        if (State != TaskState.InProgress)
        {
            throw new InvalidOperationException("Cannot complete a task that is not in the in progress state");
        }

        State = TaskState.Completed;
        CompletedAt = DateTime.UtcNow;
        AssignedUser = NullUser.Instance;
    }

    public void Suspend()
    {
        State = TaskState.Waiting;
        SuspendedAt = DateTime.UtcNow;
        AssignedUser = NullUser.Instance;
    }

    public void StartWithUser(User newUser)
    {
        AssignedUser = newUser;
        if (AssignedUser is null && State != TaskState.Waiting)
        {
            throw new InvalidOperationException("Cannot start a task that is not in the waiting state");
        }

        State = TaskState.InProgress;
        StartedAt ??= DateTime.UtcNow;
        SuspendedAt = null;
    }
    
    public void Delete()
    {
        IsDeleted = true;
        AssignedUser = NullUser.Instance;
        DeletedAt = DateTime.UtcNow;
    }
}