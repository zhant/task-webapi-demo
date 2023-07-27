using AutoMapper;
using Cnvs.Demo.TaskManagement.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Cnvs.Demo.TaskManagement.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ITaskEngine _taskEngine;
    private readonly IMapper _mapper;

    public UserController(ITaskEngine taskEngine, IMapper mapper)
    {
        _taskEngine = taskEngine;
        _mapper = mapper;
    }

    // Add user
    [HttpPost(Name = "AddUser")]
    public async Task<IActionResult> AddUser(User user)
    {
        var domainUser = _mapper.Map<Domain.User>(user);
        var result = await _taskEngine.CreateUserAsync(domainUser);
        return result.IsSuccess 
            ? Ok(_mapper.Map<User>(result.Value))
            : BadRequest(result.ErrorMessage);
    }

    // Get user
    [HttpGet("{userName}", Name = "GetUser")]
    public async Task<IActionResult> GetUser(string userName)
    {
        var result = await _taskEngine.GetUserAsync(userName);
        return result.IsSuccess 
            ? Ok(_mapper.Map<User>(result.Value))
            : BadRequest(result.ErrorMessage);
    }

    // Delete user
    [HttpDelete("delete/{userName}", Name = "DeleteUser")]
    public async Task<IActionResult> DeleteUser(string userName)
    {
        var result = await _taskEngine.DeleteUserAsync(userName);
        return result.IsSuccess 
            ? Ok(result.Value)
            : BadRequest(result.ErrorMessage);
    }

    // Get user tasks
    [HttpGet("{userName}/tasks", Name = "GetUserTasks")]
    public async Task<IActionResult> GetUserTasks(string userName)
    {
        var result = await _taskEngine.GetUserTasksAsync(userName);
        return result.IsSuccess 
            ? Ok(result.Value)
            : BadRequest(result.ErrorMessage);
    }
}