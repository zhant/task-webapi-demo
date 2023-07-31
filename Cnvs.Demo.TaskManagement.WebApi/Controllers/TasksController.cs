using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Cnvs.Demo.TaskManagement.WebApi.Controllers;

[ApiController]
[Route("taskToCreate-management/tasks")]
public class TasksController : ControllerBase
{
    private readonly ITaskEngine _taskEngine;
    private readonly IMapper _mapper;

    public TasksController(ITaskEngine taskEngine, IMapper mapper)
    {
        _taskEngine = taskEngine;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetTasks()
    {
        var result = await _taskEngine.GetTasksAsync();
        return result.IsSuccess 
            ? Ok(result.Value.Select(x => _mapper.Map<Dto.Task>(x)))
            : BadRequest(result.ErrorMessage);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTask(Guid id)
    {
        var result = await _taskEngine.GetTaskAsync(id);
        return result.IsSuccess 
            ? Ok(_mapper.Map<Dto.Task>(result.Value))
            : BadRequest(result.ErrorMessage);
    }
    
    [HttpGet("search")]
    public async Task<IActionResult> GetTaskByDescription([FromQuery] string description)
    {
        var result = await _taskEngine.GetTasksByDescriptionAsync(description);
        return result.IsSuccess 
            ? Ok(result.Value.Select(x => _mapper.Map<Dto.Task>(x)))
            : BadRequest(result.ErrorMessage);
    }

    [HttpGet("{id:guid}/users")]
    public async Task<IActionResult> GetTaskUsers(Guid id)
    {
        var result = await _taskEngine.GetTaskUsersAsync(id);
        return result.IsSuccess 
            ? Ok(result.Value.Select(x => _mapper.Map<Dto.User>(x)))
            : BadRequest(result.ErrorMessage);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask(Dto.TaskToCreate taskToCreate)
    {
        var result = await _taskEngine.CreateTaskAsync(taskToCreate.Description);
        var taskCreated = _mapper.Map<Dto.Task>(result.Value);
        return result.IsSuccess 
            ? CreatedAtAction(nameof(GetTask), new { id = taskCreated.Id }, taskCreated)
            : BadRequest(result.ErrorMessage);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateTask(Dto.Task task)
    {
        throw new NotImplementedException();
        // var result = await _taskEngine.UpdateTaskAsync(taskToCreate.Id, taskToCreate.Description);
        // return result.IsSuccess 
            // ? Ok(_mapper.Map<Dto.Task>(result.Value))
            // : BadRequest(result.ErrorMessage);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var result = await _taskEngine.DeleteTaskAsync(id);
        return result.IsSuccess 
            ? Ok(_mapper.Map<Dto.Task>(result.Value))
            : BadRequest(result.ErrorMessage);
    }
}