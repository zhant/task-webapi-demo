using Cnvs.Demo.TaskManagement.Domain;
using DomainTask = Cnvs.Demo.TaskManagement.Domain.Task;

namespace Cnvs.Demo.TaskManagement;

public interface ITaskRepository
{
    Task<Result<DomainTask>> AddTask(Domain.Task task);
    Task<Result<DomainTask>> GetTaskAsync(Guid taskId);
    Task<Result<IEnumerable<DomainTask>>> GetTasksAsync();
    Task<Result<IEnumerable<DomainTask>>> GetTasksAsync(TaskState[] taskStates);
    Task<Result<IEnumerable<DomainTask>>> GetTasksAsync(TaskState taskState);
    Task<Result<DomainTask>> UpdateTaskAsync(Domain.Task task);
    Task<Result<IEnumerable<DomainTask>>> GetUserTasksByNameAsync(string userName);
    Result<IEnumerable<DomainTask>> GetTasks();
    Task<Result<IEnumerable<DomainTask>>> GetUserTasksAsync(Guid id);
    Task<Result<IEnumerable<User>>> GetTaskUsersAsync(Guid id);
    Task<Result<string>> DeleteTaskAsync(Guid taskId);
}