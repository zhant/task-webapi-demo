using System.Collections.Concurrent;
using Cnvs.Demo.TaskManagement.Domain;
using DomainTask = Cnvs.Demo.TaskManagement.Domain.Task;
using Task = System.Threading.Tasks.Task;

namespace Cnvs.Demo.TaskManagement.Storage.InMemory;

public class TaskInMemoryRepository : ITaskRepository
{
    private readonly ConcurrentDictionary<Guid, DomainTask> _tasks = new();

    public async Task<Result<string>> DeleteTaskAsync(Guid taskId)
    {
        var success = _tasks.TryRemove(taskId, out var _);
        
        return success 
            ? Result<string>.Success(taskId.ToString())
            : Result<string>.Failure("Task not found", taskId.ToString());
    }

    public async Task<Result<DomainTask>> AddTask(DomainTask task)
    {
        _tasks[task.Id] = task;
 
        return Result<DomainTask>.Success(task);
    }

    public async Task<Result<DomainTask>> GetTaskAsync(Guid taskId)
    {
        var taskExists = _tasks.TryGetValue(taskId, out var task);
        
        return taskExists 
            ? Result<DomainTask>.Success(task!)
            : Result<DomainTask>.Failure("Task not found", NullTask.Instance);
    }

    public async Task<Result<IEnumerable<DomainTask>>> GetTasksAsync(TaskState[] taskStates)
    {
        var tasks = _tasks.Values.Where(t => taskStates.Contains(t.State)).ToList();
        
        return Result<IEnumerable<DomainTask>>.Success(tasks);
    }

    public async Task<Result<IEnumerable<DomainTask>>> GetTasksAsync(TaskState taskState)
    {
        if (!Enum.IsDefined(typeof(TaskState), taskState))
        {
            return await Task.FromResult(Result<IEnumerable<DomainTask>>.Failure("Invalid task state", Enumerable.Empty<DomainTask>()));
        }
        
        if (taskState == TaskState.Undefined)
        {
            return await GetTasksAsync();
        }

        var tasks = _tasks.Values.Where(t => t.State == taskState);

        return await Task.FromResult(Result<IEnumerable<DomainTask>>.Success(tasks.ToList()));
    }
    
    public async Task<Result<IEnumerable<DomainTask>>> GetTasksAsync()
    {
        var tasks = _tasks.Values.AsEnumerable();
        return await Task.FromResult(Result<IEnumerable<DomainTask>>.Success(tasks.ToList()));
    }

    public async Task<Result<DomainTask>> UpdateTaskAsync(DomainTask task)
    {
        _tasks[task.Id] = task;
        return Result<DomainTask>.Success(task);
    }

    public async Task<Result<IEnumerable<DomainTask>>> GetUserTasksByNameAsync(string userName)
    {
        var tasks = _tasks.Values.Where(t => t.AssignedUser.Name == userName).ToList();
        return Result<IEnumerable<DomainTask>>.Success(tasks);
    }

    public Result<IEnumerable<DomainTask>> GetTasks()
    {
        var tasks = _tasks.Values.ToList();
        return Result<IEnumerable<DomainTask>>.Success(tasks);
    }

    public async Task<Result<IEnumerable<DomainTask>>> GetUserTasksAsync(Guid id)
    {
        var tasks = _tasks.Values.Where(t => t.AssignedUser.Id == id.ToString()).ToList();
        return Result<IEnumerable<DomainTask>>.Success(tasks);
    }

    public async Task<Result<IEnumerable<User>>> GetTaskUsersAsync(Guid id)
    {
        var taskExists = _tasks.TryGetValue(id, out var task);
    
        if (!taskExists || task is null)
        {
            return Result<IEnumerable<User>>.Failure("Task not found", Enumerable.Empty<User>());
        }

        var users = task.AssignedUsersHistory.ToList();
        if (task.AssignedUser is not NullUser)
        {
            users.Add(task.AssignedUser);
        }
        
        return await Task.FromResult(Result<IEnumerable<User>>.Success(users));
    }
}