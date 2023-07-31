using System.Collections.Concurrent;
using Cnvs.Demo.TaskManagement.Domain;
using DomainTask = Cnvs.Demo.TaskManagement.Domain.Task;
using Task = System.Threading.Tasks.Task;

namespace Cnvs.Demo.TaskManagement.Storage.InMemory;

public class TaskInMemoryRepository : ITaskRepository
{
    private readonly ConcurrentDictionary<Guid, DomainTask> _tasks = new();

    public Task<Result<string>> DeleteTaskAsync(Guid taskId)
    {
        var success = _tasks.TryRemove(taskId, out var _);
        
        return success 
            ? Task.FromResult(Result<string>.Success(taskId.ToString()))
            : Task.FromResult(Result<string>.Failure("Failed to delete task", taskId.ToString()));
    }

    public Task<Result<IEnumerable<DomainTask>>> GetTasksByDescriptionAsync(string description)
    {
        var tasks = _tasks.Values.Where(x => x.Description.Contains(description,
            StringComparison.OrdinalIgnoreCase)).ToList();
        return Task.FromResult(Result<IEnumerable<DomainTask>>.Success(tasks));
    }
    
    public Task<Result<DomainTask>> AddTaskAsync(DomainTask task)
    {
        var added = _tasks.TryAdd(task.Id, task);
        return Task.FromResult(added 
            ? Result<DomainTask>.Success(task) 
            : Result<DomainTask>.Failure("Failed to add task", task));
    }

    public Task<Result<DomainTask>> GetTaskAsync(Guid taskId)
    {
        var taskExists = _tasks.TryGetValue(taskId, out var task);
        
        return taskExists 
            ? Task.FromResult(Result<DomainTask>.Success(task!))
            : Task.FromResult(Result<DomainTask>.Failure("Task not found", NullTask.Instance));
    }

    public Task<Result<IEnumerable<DomainTask>>> GetTasksAsync(TaskState[] taskStates)
    {
        var tasks = _tasks.Values.Where(t => taskStates.Contains(t.State)).ToList();
        return Task.FromResult(Result<IEnumerable<DomainTask>>.Success(tasks));
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

        var tasks = _tasks.Values.Where(t => t.State == taskState).ToList();
        return Result<IEnumerable<DomainTask>>.Success(tasks);
    }
    
    public Task<Result<IEnumerable<DomainTask>>> GetTasksAsync()
    {
        var tasks = _tasks.Values.ToList();
        return Task.FromResult(Result<IEnumerable<DomainTask>>.Success(tasks));
    }

    public Task<Result<DomainTask>> UpdateTaskAsync(DomainTask task)
    {
        _tasks[task.Id] = task;
        return Task.FromResult(Result<DomainTask>.Success(task));
    }

    public Task<Result<IEnumerable<DomainTask>>> GetUserTasksByNameAsync(string userName)
    {
        var tasks = _tasks.Values.Where(t => t.AssignedUser.Name == userName).ToList();
        return Task.FromResult(Result<IEnumerable<DomainTask>>.Success(tasks));
    }

    public Result<IEnumerable<DomainTask>> GetTasks()
    {
        var tasks = _tasks.Values.ToList();
        return Result<IEnumerable<DomainTask>>.Success(tasks);
    }

    public Task<Result<IEnumerable<DomainTask>>> GetUserTasksAsync(Guid id)
    {
        var tasks = _tasks.Values.Where(t => t.AssignedUser.Id.Equals(id.ToString())).ToList();
        return Task.FromResult(Result<IEnumerable<DomainTask>>.Success(tasks));
    }

    public Task<Result<IEnumerable<User>>> GetTaskUsersAsync(Guid id)
    {
        var taskExists = _tasks.TryGetValue(id, out var task);
    
        if (!taskExists || task is NullTask || task is null)
        {
            return Task.FromResult(Result<IEnumerable<User>>.Failure("Task not found", Enumerable.Empty<User>()));
        }

        var users = task.AssignedUsersHistory.ToList();
        if (task.AssignedUser is not NullUser)
        {
            users.Add(task.AssignedUser);
        }
        
        return Task.FromResult(Result<IEnumerable<User>>.Success(users));
    }
}