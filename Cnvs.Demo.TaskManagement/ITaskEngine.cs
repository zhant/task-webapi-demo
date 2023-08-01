using Cnvs.Demo.TaskManagement.Domain;
using Task = System.Threading.Tasks.Task;

namespace Cnvs.Demo.TaskManagement;

public interface ITaskEngine
{
    Task<Result<IEnumerable<Domain.Task>>> GetTasksAsync();
    Task<Result<Domain.Task>> GetTaskAsync(Guid id);
    Task<Result<Domain.Task>> CreateTaskAsync(string taskDescription);
    Task<Result<string>> DeleteTaskAsync(Guid id);
    Task<Result<IEnumerable<User>>> GetUsersAsync();
    Task<Result<User>> GetUserAsync(Guid id);
    Task<Result<User>> GetUserByNameAsync(string name);
    Task<Result<User>> CreateUserAsync(User user);
    Task<Result<string>> DeleteUserAsync(string name);
    Task<Result<IEnumerable<Domain.Task>>> GetUserTasksByUserNameAsync(string name);
    Task RotateTask(Domain.Task task);
    Task<Result<IEnumerable<Domain.Task>>> GetTasks(TaskState[] allowedStates);
    Task<Result<IEnumerable<User>>> GetUsers();
    Task<Result<User>> UpdateUserAsync(User domainUser);
    Task<Result<IEnumerable<Domain.Task>>> GetUserTasksByUserAsync(Guid name);
    Task<Result<IEnumerable<User>>> GetTaskUsersAsync(Guid id);
    Task<Result<IEnumerable<Domain.Task>>> GetTasksByDescriptionAsync(string description);
    Task<Result<Domain.Task>> UpdateTaskAsync(Guid id, string description);
}