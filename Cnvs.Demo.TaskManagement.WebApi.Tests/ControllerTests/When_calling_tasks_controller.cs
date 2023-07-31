using AutoMapper;
using Cnvs.Demo.TaskManagement.Domain;
using Cnvs.Demo.TaskManagement.Dto;
using Cnvs.Demo.TaskManagement.WebApi.Controllers;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Task = System.Threading.Tasks.Task;

namespace Cnvs.Demo.TaskManagement.WebApi.Tests.ControllerTests;

public class When_calling_tasks_controller
{
    private readonly ITaskEngine _fakeTaskEngine;
    private readonly IMapper _fakeMapper;
    private readonly TasksController _controller;
    
    public When_calling_tasks_controller()
    {
        _fakeTaskEngine = A.Fake<ITaskEngine>();
        _fakeMapper = A.Fake<IMapper>();
        _controller = new TasksController(_fakeTaskEngine, _fakeMapper);
    }
    
    [Fact]
    public async Task CreateTask_Succeeds()
    {
        // Arrange
        var taskToCreate = new TaskToCreate { Description = "Test task" };

        var domainTask = Domain.Task.NewTask(taskToCreate.Description);
        A.CallTo(() => _fakeTaskEngine.CreateTaskAsync(taskToCreate.Description))
            .Returns(Task.FromResult(Result<Domain.Task>.Success(domainTask)));

        var dtoTask = new Dto.Task { Id = domainTask.Id, Description = domainTask.Description };
        A.CallTo(() => _fakeMapper.Map<Dto.Task>(A<Domain.Task>._)).Returns(dtoTask);

        // Act
        var result = await _controller.CreateTask(taskToCreate);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var createdAtActionResult = (CreatedAtActionResult)result;
        createdAtActionResult.Value.Should().BeOfType<Dto.Task>().Which.Description.Should().Be(taskToCreate.Description);
    }

    [Fact]
    public async Task CreateTask_Fails()
    {
        // Arrange
        var taskToCreate = new TaskToCreate { Description = "Test task" };
        A.CallTo(() => _fakeTaskEngine.CreateTaskAsync(taskToCreate.Description))
            .Returns(Task.FromResult(Result<Domain.Task>.Failure("Error message", NullTask.Instance)));

        // Act
        var result = await _controller.CreateTask(taskToCreate);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = (BadRequestObjectResult)result;
        badRequestResult.Value.Should().Be("Error message");
    }   
}
