using Cnvs.Demo.TaskManagement.Domain;
using DomainTask = Cnvs.Demo.TaskManagement.Domain.Task;

namespace Cnvs.Demo.TaskManagement;

public class TaskEngine : ITaskEngine
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly List<User> _users;
    private readonly List<DomainTask> _tasks;
    private readonly ILogger<TaskEngine> _logger;
    private readonly IUserRandomizer _userRandomizer;

    public TaskEngine(ITaskRepository taskRepository, IUserRepository userRepository, ILogger<TaskEngine> logger, IUserRandomizer userRandomizer)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
        _logger = logger;
        _userRandomizer = userRandomizer;
        _tasks = new List<DomainTask>();
        _users = new List<User>();
    }
    
    public async System.Threading.Tasks.Task InitializeAsync()
    {
        var usersResult = await _userRepository.GetUsersAsync();
        if (usersResult.IsFailure)
        {
            _logger.LogCritical("Failed to get users: {Error}", usersResult.ErrorMessage);
            return;
        }
        
        var tasksResult = await _taskRepository.GetTasksAsync();
        if (tasksResult.IsFailure)
        {
            _logger.LogCritical("Failed to get users: {Error}", usersResult.ErrorMessage);
            return;
        }
        
        _tasks.AddRange(tasksResult.Value);
        _users.AddRange(usersResult.Value);
    }

    public IEnumerable<DomainTask> GetTasks()
    {
        return _tasks;
    }

    public void RotateTask(DomainTask task)
    {
        if (task.State == TaskState.Completed)
        {
            throw new InvalidOperationException("Cannot rotate a completed task");
        }

        User newUser;
        do
        {
            newUser = GetRandomUser(usersToExclude: task.AssignedUsersHistory);
        } while (newUser is NullUser || task.AssignedUsersHistory.Contains(newUser));

        if (newUser is NullUser)
        {
            task.State = TaskState.Waiting;
            task.AssignedUser = null;
            return;
        }
        
        task.AssignedUser = newUser;
        task.AssignedUsersHistory.Add(newUser);
        task.TransferCount++;
        if (task.TransferCount < 3)
        {
            return;
        }

        task.State = TaskState.Completed;
        task.AssignedUser = null;
    }

    public User GetRandomUser()
    {
        return _userRandomizer.GetRandomUser(_users);
    }

    private User GetRandomUser(IEnumerable<User> usersToExclude)
    {
        return _userRandomizer.GetRandomUser(_users.Except(usersToExclude));
    }

    public async Task<Result<DomainTask>> CreateTaskAsync(string taskDescription)
    {
        var randomUser = GetRandomUser();
        var assignedUser = randomUser == NullUser.Instance ? null : randomUser;
        var task = new DomainTask(Guid.NewGuid(), taskDescription)
        {
            AssignedUser = assignedUser
        };
        
        _tasks.Add(task);
        var taskResult = await _taskRepository.AddTask(task);
        return taskResult;
    }

    public async Task<Result<IEnumerable<DomainTask>>> GetTasksAsync(TaskState state = TaskState.Undefined)
    {
        throw new NotImplementedException();
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

    public async System.Threading.Tasks.Task DeleteTaskAsync(Guid taskId)
    {
        await _taskRepository.DeleteTaskAsync(taskId);
    }
    
    public async Task<Result<User>> CreateUserAsync(string userName)
    {
        var result = await _userRepository.GetUserAsync(userName);
        if (result.IsFailure)
        {
            var message = $"Failed to get user {userName}: {result.ErrorMessage}";
            _logger.LogError(message);
            return Result<User>.Failure(message, NullUser.Instance);
        }        
        
        if (result.IsSuccess && result.Value != NullUser.Instance)
        {
            return Result<User>.Failure($"User with name {userName} already exists", NullUser.Instance);
        }
        
        var user = new User(userName);
        _users.Add(user);
        var addUser = await _userRepository.AddUser(user);
        if (addUser.IsFailure)
        {
            var message = $"Failed to add user to repository: {userName}: {addUser.ErrorMessage}";
            _logger.LogError(message);
            return Result<User>.Failure(message, NullUser.Instance);
        }
        
        return Result<User>.Success(addUser.Value);
    }
    
    public async Task<Result<User>> GetUserAsync(string userName)
    {
        var result = await _userRepository.GetUserAsync(userName);
        if (result.IsFailure)
        {
            _logger.LogError("Failed to get user {UserName}: {Error}", userName, result.ErrorMessage);
        }
    
        return result;
    }
    
    public async Task<Result<IEnumerable<User>>> GetUsersAsync()
    {
        return await _userRepository.GetUsersAsync();
    }
    
    public async Task<Result<string>> DeleteUserAsync(string userName)
    {
        var result = await _userRepository.DeleteUserAsync(userName);
        return result;
    }
    
    public async Task<Result<IEnumerable<DomainTask>>> GetUserTasksAsync(string userName)
    {
        var result = await _taskRepository.GetUserTasksAsync(userName);
        return result;
    }
}