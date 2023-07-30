using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Cnvs.Demo.TaskManagement.WebApi.Controllers;

[ApiController]
[Route("tasks")]
public class TasksController : ControllerBase
{
    private readonly ITaskEngine _taskEngine;
    private readonly IMapper _mapper;

    public TasksController(ITaskEngine taskEngine, IMapper mapper)
    {
        _taskEngine = taskEngine;
        _mapper = mapper;
    }

    /// <summary>
    ///  GET Tasks
    /// </summary>
    /// <returns></returns>
    [HttpGet(Name = "GetTasks")]
    public async Task<IActionResult> GetTasks()
    {
        var result = await _taskEngine.GetTasksAsync();
        return result.IsSuccess 
            ? Ok(result.Value.Select(x => _mapper.Map<Dto.Task>(x)))
            : BadRequest(result.ErrorMessage);
    }
    
    // Add task
    [HttpPost(Name = "AddTask")]
    public async Task<IActionResult> AddTask(Dto.Task task)
    {
        var result = await _taskEngine.CreateTaskAsync(task.Description);
        return result.IsSuccess 
            ? Ok(_mapper.Map<Dto.Task>(result.Value))
            : BadRequest(result.ErrorMessage);
    }
    
    // Get task
    [HttpGet("{id:guid}", Name = "GetTask")]
    public async Task<IActionResult> GetTask(Guid id)
    {
        var result = await _taskEngine.GetTaskAsync(id);
        return result.IsSuccess 
            ? Ok(_mapper.Map<Dto.Task>(result.Value))
            : BadRequest(result.ErrorMessage);
    }
    
    // Delete task
    [HttpDelete("{id:guid}", Name = "DeleteTask")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var result = await _taskEngine.DeleteTaskAsync(id);
        return result.IsSuccess 
            ? Ok(result.Value)
            : BadRequest(result.ErrorMessage);
    }
}