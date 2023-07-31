using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using DtoUser = Cnvs.Demo.TaskManagement.Dto.User;

namespace Cnvs.Demo.TaskManagement.WebApi.Controllers;

[ApiController]
[Route("task-management/users")]
public class UsersController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ITaskEngine _taskEngine;

    public UsersController(IMapper mapper, ITaskEngine taskEngine)
    {
        _mapper = mapper;
        _taskEngine = taskEngine;
    }
        
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var result = await _taskEngine.GetUsersAsync();
        return result.IsSuccess
            ? Ok(result.Value.Select(user => _mapper.Map<DtoUser>(user)))
            : BadRequest(result.ErrorMessage);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(string id)
    {
        var result = await _taskEngine.GetUserAsync(id);
        return result.IsSuccess 
            ? Ok(_mapper.Map<DtoUser>(result.Value))
            : BadRequest(result.ErrorMessage);
    }

    [HttpGet("userName/{userName}")]
    public async Task<IActionResult> GetUserByName(string userName)
    {
        var result = await _taskEngine.GetUserByNameAsync(userName);
        return result.IsSuccess 
            ? Ok(_mapper.Map<DtoUser>(result.Value))
            : BadRequest(result.ErrorMessage);
    }

    [HttpPost]
    public async Task<IActionResult> AddUser(DtoUser userDto)
    {
        var domainUser = Domain.User.Create(userDto.Name);
        domainUser.CreatedAt = DateTime.UtcNow;
        var result = await _taskEngine.CreateUserAsync(domainUser);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetUser),
                new { id = result.Value.Id }, 
                _mapper.Map<DtoUser>(result.Value))
            : BadRequest(result.ErrorMessage);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, DtoUser userDto)
    {
        var domainUser = Domain.User.Create(userDto.Name);
        domainUser.Id = id;
        var result = await _taskEngine.UpdateUserAsync(domainUser);
        return result.IsSuccess 
            ? Ok(_mapper.Map<DtoUser>(result.Value))
            : BadRequest(result.ErrorMessage);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var result = await _taskEngine.DeleteUserAsync(id);
        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.ErrorMessage);
    }

    [HttpGet("{id:guid}/tasks")]
    public async Task<IActionResult> GetUserTasks(Guid id)
    {
        var result = await _taskEngine.GetUserTasksByUserAsync(id);
        return result.IsSuccess 
            ? Ok(result.Value.Select(task => _mapper.Map<Dto.Task>(task))) 
            : BadRequest(result.ErrorMessage);
    }
    
    [HttpGet("{userName}/tasks")]
    public async Task<IActionResult> GetUserTasksByUserName(string userName)
    {
        var result = await _taskEngine.GetUserTasksByUserNameAsync(userName);
        return result.IsSuccess 
            ? Ok(result.Value.Select(task => _mapper.Map<Dto.Task>(task))) 
            : BadRequest(result.ErrorMessage);
    }
}