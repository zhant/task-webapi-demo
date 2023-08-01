using Cnvs.Demo.TaskManagement.Configuration;
using Cnvs.Demo.TaskManagement.Domain;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DomainTask = Cnvs.Demo.TaskManagement.Domain.Task;
using Task = System.Threading.Tasks.Task;

namespace Cnvs.Demo.TaskManagement.Tests;

public class When_rotating_task
{
    private readonly TaskEngine _taskEngine;
    private readonly ITaskRepository _fakeTaskRepo;
    private readonly IUserRepository _fakeUserRepo;
    private readonly IUserRandomizer _userRandomizer;

    public When_rotating_task()
    {
        _fakeTaskRepo = A.Fake<ITaskRepository>();
        _fakeUserRepo = A.Fake<IUserRepository>();
        var fakeLogger = A.Fake<ILogger<TaskEngine>>();
        _userRandomizer = A.Fake<IUserRandomizer>();
        var options = A.Fake<IOptionsSnapshot<TaskEngineOptions>>();
        _taskEngine = new TaskEngine(_fakeTaskRepo,
            _fakeUserRepo,
            fakeLogger,
            _userRandomizer,
            options);
    }

    [Fact]
    public void RotateTask_Should_Throw_InvalidOperationException_When_Task_Is_Completed()
    {
        // Arrange
        var task = Domain.Task.NewTask("Description 1");
        task.State = TaskState.Completed;

        // Act
        var action = async () => await _taskEngine.RotateTask(task);

        // Assert
        action.Should().ThrowAsync<InvalidOperationException>().WithMessage("Cannot rotate a completed task");
    }

    [Fact]
    public async Task RotateTask_Should_Not_Assign_Same_User_Twice()
    {
        // Arrange
        var task = Domain.Task.NewTask("Description 1");
        task.AssignedUser = User.Create("User1"); 
        var users = new List<User> { User.Create("User1") };
        
        A.CallTo(() => _fakeUserRepo.GetUsersAsync()).Returns(Result<IEnumerable<User>>.Success(users));
        A.CallTo(() => _fakeTaskRepo.GetTasks()).Returns(Result<IEnumerable<DomainTask>>.Success(new List<DomainTask> { task }));
        
        // Act
        await _taskEngine.RotateTask(task);

        // Assert
        task.AssignedUser.Name.Should().NotBe("User1");
    }

    [Fact]
    public async Task RotateTask_Should_SetStateAsWaiting_When_NoUsersAvailable()
    {
        // Arrange
        var user = User.Create("User1");
        var users = new List<User> { user };
        var task = Domain.Task.NewTask("Description 1");
        task.AssignToUser(user);

        A.CallTo(() => _fakeUserRepo.GetUsersAsync()).Returns(Result<IEnumerable<User>>.Success(users));
        A.CallTo(() => _fakeTaskRepo.GetTasks()).Returns(Result<IEnumerable<DomainTask>>.Success(new List<DomainTask> { task }));
        A.CallTo(() => _userRandomizer.GetRandomUser(A<IEnumerable<User>>._)).Returns(NullUser.Instance);
        // Act
        await _taskEngine.RotateTask(task);

        // Assert
        task.State.Should().Be(TaskState.Waiting);
        task.AssignedUser.Should().BeOfType(NullUser.Instance.GetType());
    }
}