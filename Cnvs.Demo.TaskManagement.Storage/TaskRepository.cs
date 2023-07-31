using Cnvs.Demo.TaskManagement.Domain;
using Microsoft.Extensions.Logging;
using DomainTask = Cnvs.Demo.TaskManagement.Domain.Task;
using Task = System.Threading.Tasks.Task;

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

    public async Task<Result<IEnumerable<DomainTask>>> GetTasksByDescriptionAsync(string description)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<DomainTask>> AddTaskAsync(DomainTask task)
    {
        _logger.LogInformation("Adding task {TaskId}", task.Id);
        throw new NotImplementedException();
    }

    public Result<DomainTask> AddTask(DomainTask task)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<DomainTask>> GetTaskAsync(Guid taskId)
    {
        _logger.LogInformation("Getting task {TaskId}", taskId);
        throw new NotImplementedException();
    }

    public async Task<Result<IEnumerable<DomainTask>>> GetTasksAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Result<IEnumerable<DomainTask>>> GetTasksAsync(TaskState[] taskStates)
    {
        _logger.LogInformation("Getting tasks");
        // TODO: Implement this Reading from the database
        var tasks = Enumerable.Empty<DomainTask>().ToList();
        _logger.LogInformation("Found {TaskCount} tasks", tasks.Count);

        return await Task.FromResult(Result<IEnumerable<DomainTask>>.Success(tasks));
    }

    public async Task<Result<IEnumerable<DomainTask>>> GetTasksAsync(TaskState taskState)
    {
        _logger.LogInformation("Getting tasks");
        throw new NotImplementedException();
    }

    public async Task<Result<DomainTask>> UpdateTaskAsync(DomainTask task)
    {
        _logger.LogInformation("Updating task {TaskId}", task.Id);
        throw new NotImplementedException();
    }

    public async Task<Result<IEnumerable<DomainTask>>> GetUserTasksByNameAsync(string userName)
    {
        _logger.LogInformation("Getting tasks for user {UserName}", userName);
        throw new NotImplementedException();
    }

    public Result<IEnumerable<DomainTask>> GetTasks()
    {
        _logger.LogInformation("Getting tasks");
        throw new NotImplementedException();

        // TODO: Implement this Reading from the database
        var tasks = Enumerable.Empty<DomainTask>();
        _logger.LogInformation("Found {TaskCount} tasks", tasks.Count());

        return Result<IEnumerable<Domain.Task>>.Success(tasks);
    }

    public async Task<Result<IEnumerable<DomainTask>>> GetUserTasksAsync(Guid id)
    {
        _logger.LogInformation("Getting tasks for user {UserId}", id);
        throw new NotImplementedException();

        // TODO: Implement this Reading from the database
    }

    public async Task<Result<IEnumerable<User>>> GetTaskUsersAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}
