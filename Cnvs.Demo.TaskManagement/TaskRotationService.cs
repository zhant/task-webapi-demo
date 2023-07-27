using Cnvs.Demo.TaskManagement.Domain;
using DomainTask = System.Threading.Tasks.Task;

namespace Cnvs.Demo.TaskManagement;

public class TaskRotationService : BackgroundService
{
    private readonly ILogger<TaskRotationService> _logger;

    private readonly ITaskEngine _taskEngine;
    private Timer? _timer;

    public TaskRotationService(ITaskEngine taskEngine, ILogger<TaskRotationService> logger)
    {
        _taskEngine = taskEngine;
        _logger = logger;
    }

    protected override async DomainTask ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Task rotation service is starting");
        
        _timer = new Timer(RotateTasks, null, TimeSpan.Zero, TimeSpan.FromMinutes(2));
        stoppingToken.Register(() => _timer.Dispose());

        while (!stoppingToken.IsCancellationRequested)
        {
            await DomainTask.Delay(TimeSpan.FromMinutes(2), stoppingToken);
        }
    }

    public override async DomainTask StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Task rotation service is stopping");
        await base.StopAsync(cancellationToken);       
    }

    public void RotateTasks(object? state)
    {
        var usersAsync = _taskEngine.GetUsersAsync().Result;
        if (!usersAsync.IsSuccess)
        {
            _logger.LogError("Failed to get users for rotation: {Error}", usersAsync.ErrorMessage);
            return;
        }
        
        var users = usersAsync.Value.ToArray();
        if (users.Length < 3)
        {
            _logger.LogInformation("Not enough users found for rotation: [{UsersLength}]", users.Length);
            return;
        }
        
        var tasks = _taskEngine.GetTasks().Where(x => x.State == TaskState.InProgress).ToArray();
        foreach (var task in tasks)
        {
            _taskEngine.RotateTask(task);
        }
        
        _logger.LogInformation("Tasks of count [{Count}] rotated successfully", tasks.Length);
    }
}