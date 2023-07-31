using Cnvs.Demo.TaskManagement.Configuration;
using Cnvs.Demo.TaskManagement.Domain;
using Microsoft.Extensions.Options;
using SystemTask = System.Threading.Tasks.Task;

namespace Cnvs.Demo.TaskManagement;

public class TaskRotationService : BackgroundService
{
    private readonly int _rotationTimeMinutes;
    private readonly ILogger<TaskRotationService> _logger;
    private readonly ITaskEngine _taskEngine;
    private Timer? _timer;

    public TaskRotationService(ITaskEngine taskEngine,
        ILogger<TaskRotationService> logger,
        IOptions<TaskEngineOptions> options)
    {
        _taskEngine = taskEngine;
        _logger = logger;
        _rotationTimeMinutes = options.Value.RotationTimeMinutes;
    }

    protected override async SystemTask ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Task rotation service is starting");

        _timer = new Timer(state =>
        {
            SystemTask.Run(async () =>
            {
                await RotateTasks(state);
            }, stoppingToken);
        }, null, TimeSpan.Zero, TimeSpan.FromMinutes(_rotationTimeMinutes));
        
        stoppingToken.Register(() => _timer.Dispose());

        while (!stoppingToken.IsCancellationRequested)
        {
            await SystemTask.Delay(TimeSpan.FromMinutes(_rotationTimeMinutes), stoppingToken);
        }
    }

    public override async SystemTask StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Task rotation service is stopping");
        await base.StopAsync(cancellationToken);       
    }

    private async SystemTask RotateTasks(object? state)
    {
        var usersResult = await _taskEngine.GetUsers();
        if (!usersResult.IsSuccess)
        {
            _logger.LogError("Failed to get users for rotation: {Error}", usersResult.ErrorMessage);
            return;
        }
        
        var users = usersResult.Value.ToArray();
 
        if (!users.Any())
        {
            _logger.LogInformation("Not enough users found for rotation: [{UsersLength}]", users.Length);
            return;
        }
        
        // We select also waiting tasks if they exist to assign them to the new user
        var result = _taskEngine.GetTasks(new[] { TaskState.Waiting, TaskState.InProgress }).Result;
        if (!result.IsSuccess)
        {
            _logger.LogError("Failed to get tasks for rotation: {Error}", result.ErrorMessage);
            return;
        }

        var tasks = result.Value.ToArray();
        foreach (var task in tasks)
        {
            await _taskEngine.RotateTask(task);
        }
        
        _logger.LogInformation("Tasks of count [{Count}] rotated successfully", tasks.Length);
    }
}