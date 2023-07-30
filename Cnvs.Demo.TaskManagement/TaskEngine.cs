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
        var usersResult = _userRepository.GetUsers();
        if (usersResult.IsFailure)
        {
            _logger.LogCritical("Failed to get users: {Error}", usersResult.ErrorMessage);
            return;
        }
        
        var tasksResult = _taskRepository.GetTasks();
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
        var usersToExclude = task.AssignedUsersHistory;
        if (task.AssignedUser is not null)
        {
            usersToExclude = usersToExclude.Append(task.AssignedUser).ToList();
        }
        
        do
        {
            newUser = GetRandomUser(usersToExclude: usersToExclude);
            if (newUser is NullUser)
            {
                break;
            }
        } while (usersToExclude.Contains(newUser));

        task.AssignedUser = newUser;
        if (newUser is NullUser)
        {
            task.State = TaskState.Waiting;
            return;
        }
        
        task.AssignedUsersHistory.Add(newUser);
        task.TransferCount++;
        if (task.TransferCount < 3)
        {
            return;
        }

        task.State = TaskState.Completed;
        task.AssignedUser = null;
    }

    private User GetRandomUser()
    {
        return _userRandomizer.GetRandomUser(_users);
    }

    private User GetRandomUser(IEnumerable<User> usersToExclude)
    {
        return _userRandomizer.GetRandomUser(_users.Except(usersToExclude, new UserEqualityComparer()));
    }
    
    public async Task<Result<DomainTask>> CreateTaskAsync(string taskDescription)
    {
        var randomUser = GetRandomUser();
        var assignedUser = randomUser == NullUser.Instance ? null : randomUser;
        var task = Domain.Task.NewTask("Description 1");
        task.AssignedUser = assignedUser; 
        
        _tasks.Add(task);
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

    public Result<IEnumerable<DomainTask>> GetTasks(TaskState[] allowedStates)
    {
        // NB in case of > 1 pods always get tasks from DB
        var tasks = _tasks.Where(x => allowedStates.Contains(x.State));
        return Result<IEnumerable<DomainTask>>.Success(tasks);
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

    public Result<IEnumerable<DomainTask>> GetTasks(TaskState state)
    {
        var r = state != TaskState.Undefined
            ? _tasks.Where(t => t.State == state)
            : _tasks;
        return Result<IEnumerable<DomainTask>>.Success(r);
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
        return Result<IEnumerable<DomainTask>>.Success(_tasks);
        
        // TODO Get tasks from DB
        var result = _taskRepository.GetTasks();
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
    
    public async Task<Result<User>> CreateUserAsync(User userToCreate)
    {
        var userName = userToCreate.Name;
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
        
        var user = User.Create(userName);
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
    
    public Result<IEnumerable<User>> GetUsers()
    {
        return Result<IEnumerable<User>>.Success(_users);
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
    
    public async Task<Result<IEnumerable<DomainTask>>> GetUserTasksAsync(string userName)
    {
        var result = await _taskRepository.GetUserTasksAsync(userName);
        return result;
    }
}