namespace Cnvs.Demo.TaskManagement.Domain;

public class Task
{
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
    
    private protected Task(Guid taskId, string taskDescription)
    {
        if (string.IsNullOrWhiteSpace(taskDescription))
        {
            throw new ArgumentException(nameof(taskDescription));
        }
        
        Id = taskId;
        Description = taskDescription;
    }

    private Task(string taskDescription)
    {
        if (string.IsNullOrWhiteSpace(taskDescription))
        {
            throw new ArgumentException(nameof(taskDescription));
        }
        
        Id = Guid.NewGuid();
        Description = taskDescription;
    }

    public static Task NewTask(string taskDescription)
    {
        return new Task(taskDescription)
        {
            State = TaskState.Waiting,
            CreatedAt = DateTime.UtcNow,
            AssignedUser = NullUser.Instance
        };
    }

    public void Complete()
    {
        if (State != TaskState.InProgress)
        {
            throw new InvalidOperationException("Cannot complete a task that is not in the in progress state");
        }

        State = TaskState.Completed;
        CompletedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
        AssignedUser = NullUser.Instance;
    }

    public void Suspend()
    {
        State = TaskState.Waiting;
        SuspendedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
        AssignedUser = NullUser.Instance;
    }

    public void AssignToUser(User newUser)
    {
        if (newUser is NullUser)
        {
            throw new InvalidOperationException("Cannot start a task with a null user");
        }

        AssignedUser = newUser;
        State = TaskState.InProgress;
        StartedAt ??= DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
        SuspendedAt = null;
        AssignedUsersHistory.Add(newUser);
        TransferCount++;
    }
    
    public void Delete()
    {
        IsDeleted = true;
        AssignedUser = NullUser.Instance;
        DeletedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
    }
}