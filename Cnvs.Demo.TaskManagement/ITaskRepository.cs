﻿using Cnvs.Demo.TaskManagement.Domain;
using DomainTask = Cnvs.Demo.TaskManagement.Domain.Task;

namespace Cnvs.Demo.TaskManagement;

public interface ITaskRepository
{
    Task<Result<string>> DeleteTaskAsync(Guid taskId);  
    Task<Result<DomainTask>> AddTask(Domain.Task task);
    Task<Result<Domain.Task>> GetTaskAsync(Guid taskId);
    Task<Result<IEnumerable<DomainTask>>> GetTasksAsync(TaskState[] taskStates);
    Task<Result<IEnumerable<DomainTask>>> GetTasksAsync(TaskState taskState);
    Task<Result<Domain.Task>> UpdateTaskAsync(Domain.Task task);
    Task<Result<IEnumerable<Domain.Task>>> GetUserTasksAsync(string userName);
    Task<Result<IEnumerable<DomainTask>>> GetTasksAsync();
}