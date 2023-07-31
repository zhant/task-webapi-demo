﻿using Cnvs.Demo.TaskManagement.Configuration;
using Cnvs.Demo.TaskManagement.Domain;
using Microsoft.Extensions.Options;
using DomainTask = Cnvs.Demo.TaskManagement.Domain.Task;

namespace Cnvs.Demo.TaskManagement;

public class TaskEngine : ITaskEngine
{
    private readonly int _changesBetweenUsers;
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<TaskEngine> _logger;
    private readonly IUserRandomizer _userRandomizer;

    public TaskEngine(ITaskRepository taskRepository,
        IUserRepository userRepository,
        ILogger<TaskEngine> logger,
        IUserRandomizer userRandomizer,
        IOptionsSnapshot<TaskEngineOptions> options)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
        _logger = logger;
        _userRandomizer = userRandomizer;
        _changesBetweenUsers = options.Value.ChangesBetweenUsers;
    }
    
    public void RotateTask(DomainTask task)
    {
        if (task.State == TaskState.Completed)
        {
            throw new InvalidOperationException("Cannot rotate a completed task");
        }

        User newUser;
        var usersToExclude = task.AssignedUsersHistory;
        
        do
        {
            newUser = GetRandomUser(usersToExclude: usersToExclude);
            if (newUser is NullUser)
            {
                break;
            }
        } while (usersToExclude.Contains(newUser));

        if (newUser is NullUser)
        {
            task.Suspend();
            return;
        }
        
        task.StartWithUser(newUser);
        
        if (task.TransferCount < _changesBetweenUsers)
        {
            return;
        }

        task.Complete();
    }

    private User GetRandomUser()
    {
        var result = GetUsers();
        if (result.IsFailure)
        {
            _logger.LogError("Failed to get users: {Error}", result.ErrorMessage);
            return NullUser.Instance;
        }
        
        return _userRandomizer.GetRandomUser(result.Value);
    }

    private User GetRandomUser(IEnumerable<User> usersToExclude)
    {
        var result = GetUsers();
        if (result.IsFailure)
        {
            _logger.LogError("Failed to get users: {Error}", result.ErrorMessage);
            return NullUser.Instance;
        }
        
        return _userRandomizer.GetRandomUser(result.Value.Except(usersToExclude, new UserEqualityComparer()));
    }
    
    public async Task<Result<DomainTask>> CreateTaskAsync(string taskDescription)
    {
        if (string.IsNullOrWhiteSpace(taskDescription))
        {
            throw new ArgumentException("Description cannot be empty");
        }

        var assignedUser = GetRandomUser();
        var task = Domain.Task.NewTask(taskDescription);
        task.AssignedUser = assignedUser; 
        
        var taskResult = await _taskRepository.AddTask(task);
        return taskResult;
    }

    public async Task<Result<IEnumerable<DomainTask>>> GetTasksAsync(TaskState[] allowedStates)
    {
        var result = await _taskRepository.GetTasksAsync(allowedStates);
        if (result.IsFailure)
        {
            _logger.LogError("Failed to get tasks: {Error}", result.ErrorMessage);
        }
        
        return result;
    }

    public async Task<Result<IEnumerable<DomainTask>>> GetTasks(TaskState[] allowedStates)
    {
        // NB in case of > 1 pods always get tasks from DB
        var result = await _taskRepository.GetTasksAsync(allowedStates);
        return result.IsSuccess
            ? Result<IEnumerable<DomainTask>>.Success(result.Value)
            : Result<IEnumerable<DomainTask>>.Failure(result.ErrorMessage, Enumerable.Empty<DomainTask>());
    }

    public async Task<Result<IEnumerable<DomainTask>>> GetTasksAsync(TaskState state)
    {
        var result = await _taskRepository.GetTasksAsync(state);
        if (result.IsFailure)
        {
            _logger.LogError("Failed to get tasks: {Error}", result.ErrorMessage);
        }
        
        return result;
    }

    public async Task<Result<DomainTask>> GetTaskAsync(Guid taskId)
    {
        var result = await _taskRepository.GetTaskAsync(taskId);
        if (result.IsFailure)
        {
            _logger.LogError("Failed to get task {TaskId}: {Error}", taskId, result.ErrorMessage);
            return result;
        }
        
        return result;
    }

    public async Task<Result<IEnumerable<DomainTask>>> GetTasksAsync()
    {
        var result = await _taskRepository.GetTasksAsync();
        if (result.IsFailure)
        {
            _logger.LogError("Failed to get tasks: {Error}", result.ErrorMessage);
        }
        
        return result;
    }

    public async Task<Result<DomainTask>> UpdateTaskAsync(DomainTask task)
    {
        var result = await _taskRepository.UpdateTaskAsync(task);
        if (result.IsFailure)
        {
            _logger.LogError("Failed to update task {TaskId}: {Error}", task.Id, result.ErrorMessage);
        }
        
        return result;
    }

    public async Task<Result<string>> DeleteTaskAsync(Guid taskId)
    {
        var result = await _taskRepository.GetTaskAsync(taskId);
        if(result.IsFailure)
        {
            _logger.LogError("Failed to get task {TaskId}: {Error}", taskId, result.ErrorMessage);
            return Result<string>.Failure(result.ErrorMessage, taskId.ToString());
        }
        
        var task = result.Value;
        task.Delete();
        await _taskRepository.UpdateTaskAsync(task);
        return Result<string>.Success(taskId.ToString());
    }
    
    public async Task<Result<User>> CreateUserAsync(User userToCreate)
    {
        var userName = userToCreate.Name;
        var result = await _userRepository.GetUserByNameAsync(userName);
        if (result.IsFailure)
        {
            var message = $"Failed to get user {userName}: {result.ErrorMessage}";
            _logger.LogError(message);
            return Result<User>.Failure(message, NullUser.Instance);
        }        
        
        if (result.IsSuccess && !NullUser.Instance.Equals(result.Value))
        {
            return Result<User>.Failure($"User with name {userName} already exists", NullUser.Instance);
        }
        
        var user = User.Create(userName);
        var addedUser = await _userRepository.AddUser(user);
        if (addedUser.IsFailure)
        {
            var message = $"Failed to add user to repository: {userName}: {addedUser.ErrorMessage}";
            _logger.LogError(message);
            return Result<User>.Failure(message, NullUser.Instance);
        }
        
        return Result<User>.Success(addedUser.Value);
    }
    
    public async Task<Result<User>> GetUserByNameAsync(string userName)
    {
        var result = await _userRepository.GetUserByNameAsync(userName);
        if (result.IsFailure)
        {
            _logger.LogError("Failed to get user {UserName}: {Error}", userName, result.ErrorMessage);
        }
    
        return result;
    }
    
    public Result<IEnumerable<User>> GetUsers()
    {
        var result = _userRepository.GetUsers();
        
        return result.IsSuccess 
            ? Result<IEnumerable<User>>.Success(result.Value)
            : Result<IEnumerable<User>>.Failure(result.ErrorMessage, Enumerable.Empty<User>());
    }

    public async Task<Result<User>> GetUserAsync(Guid id)
    {
        var result = await _userRepository.GetUserAsync(id);
        if (result.IsFailure)
        {
            _logger.LogError("Failed to get user {Id}: {Error}", id, result.ErrorMessage);
        }
    
        return result;
    }

    public async Task<Result<User>> UpdateUserAsync(User domainUser)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<IEnumerable<DomainTask>>> GetUserTasksByUserAsync(Guid id)
    {
        var result = await _taskRepository.GetUserTasksAsync(id);
        return result;
    }

    public async Task<Result<IEnumerable<User>>> GetTaskUsersAsync(Guid id)
    {
        return await _taskRepository.GetTaskUsersAsync(id);
    }

    public async Task<Result<IEnumerable<User>>> GetUsersAsync()
    {
        return _userRepository.GetUsers();
    }
    
    public async Task<Result<string>> DeleteUserAsync(string userName)
    {
        var result = await _userRepository.DeleteUserAsync(userName);
        return result;
    }
    
    public async Task<Result<IEnumerable<DomainTask>>> GetUserTasksByUserNameAsync(string userName)
    {
        var result = await _taskRepository.GetUserTasksByNameAsync(userName);
        return result;
    }
}