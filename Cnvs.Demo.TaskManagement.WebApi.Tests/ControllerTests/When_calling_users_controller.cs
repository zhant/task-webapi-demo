using AutoMapper;
using Cnvs.Demo.TaskManagement.Domain;
using Cnvs.Demo.TaskManagement.Dto;
using Cnvs.Demo.TaskManagement.WebApi.Controllers;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Task = System.Threading.Tasks.Task;
using User = Cnvs.Demo.TaskManagement.Dto.User;

namespace Cnvs.Demo.TaskManagement.WebApi.Tests.ControllerTests;


public class When_calling_users_controller
{
    private readonly ITaskEngine _fakeTaskEngine;
    private readonly IMapper _fakeMapper;
    private readonly UsersController _controller;

    public When_calling_users_controller()
    {
        _fakeTaskEngine = A.Fake<ITaskEngine>();
        _fakeMapper = A.Fake<IMapper>();
        _controller = new UsersController(_fakeMapper, _fakeTaskEngine);
    }

    [Fact]
    public async Task AddUser_Succeeds()
    {
        // Arrange
        var userDto = new UserToCreate { Name = "Test User" };
        var domainUser = Domain.User.Create(userDto.Name);
        domainUser.CreatedAt = DateTime.UtcNow;
        A.CallTo(() => _fakeTaskEngine.CreateUserAsync(A<Domain.User>._))
            .Returns(Task.FromResult(Result<Domain.User>.Success(domainUser)));

        var dtoUser = new User { Id = domainUser.Id, Name = domainUser.Name };
        A.CallTo(() => _fakeMapper.Map<User>(A<Domain.User>._)).Returns(dtoUser);

        // Act
        var result = await _controller.AddUser(userDto);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var createdAtActionResult = (CreatedAtActionResult)result;
        createdAtActionResult.Value.Should().BeOfType<User>().Which.Name.Should().Be(userDto.Name);
    }

    [Fact]
    public async Task AddUser_Fails()
    {
        // Arrange
        var userDto = new UserToCreate { Name = "Test User" };
        A.CallTo(() => _fakeTaskEngine.CreateUserAsync(A<Domain.User>._))
            .Returns(Task.FromResult(Result<Domain.User>.Failure("Error message", NullUser.Instance)));

        // Act
        var result = await _controller.AddUser(userDto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = (BadRequestObjectResult)result;
        badRequestResult.Value.Should().Be("Error message");
    }
}