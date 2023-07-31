﻿using Cnvs.Demo.TaskManagement.Domain;
using Task = System.Threading.Tasks.Task;

namespace Cnvs.Demo.TaskManagement;

public interface ITaskEngine
{
    Task<Result<IEnumerable<Domain.Task>>> GetTasksAsync(TaskState state = TaskState.Undefined);

    Task<Result<Domain.Task>> GetTaskAsync(Guid id);
    
    Task<Result<Domain.Task>> CreateTaskAsync(string taskDescription);
    Task<Result<string>> DeleteTaskAsync(Guid id);
    
    Task<Result<IEnumerable<User>>> GetUsersAsync() => 
        Task.FromResult(Result<IEnumerable<User>>.Success(Enumerable.Empty<User>()));

    Task<Result<User>> GetUserByNameAsync(string name);
    Task<Result<User>> CreateUserAsync(User user) => Task.FromResult(Result<User>.Success(user));

    Task<Result<string>> DeleteUserAsync(string name) => Task.FromResult(Result<string>.Success(name));
    
    Task<Result<IEnumerable<Domain.Task>>> GetUserTasksByUserNameAsync(string name);

    void RotateTask(Domain.Task task);
    Task<Result<IEnumerable<Domain.Task>>> GetTasksAsync(TaskState[] allowedStates);
    Task<Result<IEnumerable<Domain.Task>>> GetTasks(TaskState[] allowedStates);
    Result<IEnumerable<User>> GetUsers();
    Task<Result<User>> GetUserAsync(Guid id);
    Task<Result<User>> UpdateUserAsync(User domainUser);
    Task<Result<IEnumerable<Domain.Task>>> GetUserTasksByUserAsync(Guid name);
    Task<Result<IEnumerable<User>>> GetTaskUsersAsync(Guid id);
}