using System.Collections.Immutable;
using Cnvs.Demo.TaskManagement.Domain;
using Microsoft.Extensions.Logging;
using Task = Cnvs.Demo.TaskManagement.Domain.Task;

namespace Cnvs.Demo.TaskManagement.Storage;

public class TaskRepository : ITaskRepository
{
    private readonly ILogger<TaskRepository> _logger;

    public TaskRepository(ILogger<TaskRepository> logger)
    {
        _logger = logger;
    }

    public async Task<Result<string>> DeleteTaskAsync(Guid taskId)
    {
        _logger.LogInformation("Deleting task {TaskId}", taskId);
        throw new NotImplementedException();
    }

    public async Task<Result<Task>> AddTask(Task task)
    {
        _logger.LogInformation("Adding task {TaskId}", task.Id);
        throw new NotImplementedException();
    }

    public async Task<Result<Task>> GetTaskAsync(Guid taskId)
    {
        _logger.LogInformation("Getting task {TaskId}", taskId);
        throw new NotImplementedException();
    }

    public async Task<Result<IEnumerable<Task>>> GetTasksAsync(TaskState[] taskStates)
    {
        _logger.LogInformation("Getting tasks");
        // TODO: Implement this Reading from the database
        var tasks = ImmutableList<Task>.Empty;
        _logger.LogInformation("Found {TaskCount} tasks", tasks.Count);

        return Result<IEnumerable<Task>>.Success(tasks);
    }

    public async Task<Result<IEnumerable<Task>>> GetTasksAsync(TaskState taskState)
    {
        _logger.LogInformation("Getting tasks");
        throw new NotImplementedException();
    }

    public async Task<Result<Task>> UpdateTaskAsync(Task task)
    {
        _logger.LogInformation("Updating task {TaskId}", task.Id);
        throw new NotImplementedException();
    }

    public async Task<Result<IEnumerable<Task>>> GetUserTasksAsync(string userName)
    {
        _logger.LogInformation("Getting tasks for user {UserName}", userName);
        throw new NotImplementedException();
    }

    public Result<IEnumerable<Task>> GetTasks()
    {
        _logger.LogInformation("Getting tasks");
        throw new NotImplementedException();

        // TODO: Implement this Reading from the database
        var tasks = ImmutableList<Task>.Empty;
        _logger.LogInformation("Found {TaskCount} tasks", tasks.Count);

        return Result<IEnumerable<Domain.Task>>.Success(tasks);
    }
}
