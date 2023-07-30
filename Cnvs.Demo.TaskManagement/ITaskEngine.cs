using Cnvs.Demo.TaskManagement.Domain;
using Task = System.Threading.Tasks.Task;

namespace Cnvs.Demo.TaskManagement;

public interface ITaskEngine
{
    Task<Result<IEnumerable<Domain.Task>>> GetTasksAsync(TaskState state = TaskState.Undefined);
    Result<IEnumerable<Domain.Task>> GetTasks(TaskState state);

    Task<Result<Domain.Task>> GetTaskAsync(Guid id);
    
    Task<Result<Domain.Task>> CreateTaskAsync(string taskDescription);
    Task<Result<string>> DeleteTaskAsync(Guid id) => Task.FromResult(Result<string>.Success(id.ToString()));
    
    Task<Result<IEnumerable<User>>> GetUsersAsync() => 
        Task.FromResult(Result<IEnumerable<User>>.Success(Enumerable.Empty<User>()));

    Task<Result<User>> GetUserAsync(string name) => Task.FromResult(Result<User>.Success(new User(name)));
    Task<Result<User>> CreateUserAsync(User user) => Task.FromResult(Result<User>.Success(user));

    Task<Result<string>> DeleteUserAsync(string name) => Task.FromResult(Result<string>.Success(name));
    
    Task<Result<IEnumerable<Domain.Task>>> GetUserTasksAsync(string name) => 
        Task.FromResult(Result<IEnumerable<Domain.Task>>.Success(Enumerable.Empty<Domain.Task>()));

    void RotateTask(Domain.Task task);
    IEnumerable<Domain.Task> GetTasks();
    Task<Result<IEnumerable<Domain.Task>>> GetTasksAsync(TaskState[] allowedStates);
    Result<IEnumerable<Domain.Task>> GetTasks(TaskState[] allowedStates);
    Result<IEnumerable<User>> GetUsers();
}