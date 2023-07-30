using System.Collections.Concurrent;
using System.Collections.Immutable;
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
        var tasks = _tasks.Values.Where(t => t.State == taskState).ToList();

        return Result<IEnumerable<DomainTask>>.Success(tasks);
    }

    public async Task<Result<DomainTask>> UpdateTaskAsync(DomainTask task)
    {
        _tasks[task.Id] = task;
        return Result<DomainTask>.Success(task);
    }

    public async Task<Result<IEnumerable<DomainTask>>> GetUserTasksByNameAsync(string userName)
    {
        var tasks = _tasks.Values.Where(t => t.AssignedUser?.Name == userName).ToList();

        return tasks.Any() 
            ? Result<IEnumerable<DomainTask>>.Success(tasks) 
            : Result<IEnumerable<DomainTask>>.Failure("No tasks found for user", ImmutableList<DomainTask>.Empty);
    }

    public Result<IEnumerable<DomainTask>> GetTasks()
    {
        var tasks = _tasks.Values.ToList();

        return tasks.Any()
            ? Result<IEnumerable<DomainTask>>.Success(tasks) 
            : Result<IEnumerable<DomainTask>>.Failure("No tasks found", ImmutableList<DomainTask>.Empty);
    }

    public async Task<Result<IEnumerable<DomainTask>>> GetUserTasksAsync(string id)
    {
        var tasks = _tasks.Values.Where(t => t.AssignedUser?.Id == id).ToList();

        return tasks.Any() 
            ? Result<IEnumerable<DomainTask>>.Success(tasks) 
            : Result<IEnumerable<DomainTask>>.Failure("No tasks found for user", ImmutableList<DomainTask>.Empty);
    }
}