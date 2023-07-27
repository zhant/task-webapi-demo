using Task = Cnvs.Demo.TaskManagement.Domain.Task;

namespace Cnvs.Demo.TaskManagement.Storage;

public class TaskRepository : ITaskRepository
{
    public async Task<Result<string>> DeleteTaskAsync(Guid taskId)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<Task>> AddTask(Task task)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<Task>> GetTaskAsync(Guid taskId)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<IEnumerable<Task>>> GetTasksAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Result<Task>> UpdateTaskAsync(Task task)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<IEnumerable<Task>>> GetUserTasksAsync(string userName)
    {
        throw new NotImplementedException();
    }
}