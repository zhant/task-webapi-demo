﻿using AutoMapper;
using Cnvs.Demo.TaskManagement.Domain;
using Cnvs.Demo.TaskManagement.Dto;
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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var result = await _taskEngine.GetUserAsync(id);
        return result.IsSuccess 
            ? result.Value is not NullUser 
                ? Ok(_mapper.Map<DtoUser>(result.Value)) 
                : NotFound("User not found")
            : BadRequest(result.ErrorMessage);
    }

    [HttpGet("search")]
    public async Task<IActionResult> GetUserByName([FromQuery] string userName)
    {
        var result = await _taskEngine.GetUserByNameAsync(userName);
        return result.IsSuccess 
            ? result.Value is not NullUser 
                ? Ok(new[] { _mapper.Map<DtoUser>(result.Value) })
                : NotFound("User not found")
            : BadRequest(result.ErrorMessage);
    }

    [HttpPost]
    public async Task<IActionResult> AddUser(UserToCreate userDto)
    {
        var domainUser = Domain.User.Create(userDto.Name);
        var result = await _taskEngine.CreateUserAsync(domainUser);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetUser),
                new { id = result.Value.Id }, 
                _mapper.Map<DtoUser>(result.Value))
            : result.Value is not NullUser
                ? Conflict(result.ErrorMessage)
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

    [HttpDelete("{userName}")]
    public async Task<IActionResult> DeleteUser(string userName)
    {
        var result = await _taskEngine.DeleteUserAsync(userName);
        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.ErrorMessage);
    }

    [HttpGet("{id:guid}/tasks")]
    public async Task<IActionResult> GetUserTasks(Guid id)
    {
        var user = await _taskEngine.GetUserAsync(id);
        if (user.Value is NullUser)
        {
            return NotFound($"User not found: {id}");
        }
        
        var result = await _taskEngine.GetUserTasksByUserAsync(id);
        return result.IsSuccess 
            ? Ok(result.Value.Select(task => _mapper.Map<Dto.Task>(task))) 
            : BadRequest(result.ErrorMessage);
    }
    
    [HttpGet("{userName}/tasks")]
    public async Task<IActionResult> GetUserTasksByUserName(string userName)
    {
        var user = await _taskEngine.GetUserByNameAsync(userName);
        
        if (user.Value is NullUser)
        {
            return NotFound($"User not found: {userName}");
        }
        
        var result = await _taskEngine.GetUserTasksByUserNameAsync(userName);
        return result.IsSuccess 
            ? Ok(result.Value.Select(task => _mapper.Map<Dto.Task>(task))) 
            : BadRequest(result.ErrorMessage);
    }
}