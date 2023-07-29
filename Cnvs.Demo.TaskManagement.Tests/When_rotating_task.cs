using Cnvs.Demo.TaskManagement.Domain;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using DomainTask = Cnvs.Demo.TaskManagement.Domain.Task;

namespace Cnvs.Demo.TaskManagement.Tests;

public class When_rotating_task
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<TaskEngine> _logger;
    private readonly IUserRandomizer _userRandomizer;
    private TaskEngine _taskEngine = null!;

    public When_rotating_task()
    {
        _taskRepository = A.Fake<ITaskRepository>();
        _userRepository = A.Fake<IUserRepository>();
        _logger = A.Fake<ILogger<TaskEngine>>();
        _userRandomizer = new UserRandomizer();
    }

    [Fact]
    public void RotateTask_Should_Throw_InvalidOperationException_When_Task_Is_Completed()
    {
        // Arrange
        _taskEngine = new TaskEngine(_taskRepository, _userRepository, _logger, _userRandomizer);
        var task = Domain.Task.NewTask("Description 1");
        task.State = TaskState.Completed;

        // Act
        var action = () => _taskEngine.RotateTask(task);

        // Assert
        action.Should().Throw<InvalidOperationException>().WithMessage("Cannot rotate a completed task");
    }

    [Fact]
    public void RotateTask_Should_Not_Assign_Same_User_Twice()
    {
        // Arrange
        var task = Domain.Task.NewTask("Description 1");
        task.AssignedUser = new User("User1"); 
        var users = new List<User> { new User("User1") };
        
        A.CallTo(() => _userRepository.GetUsers()).Returns(Result<IEnumerable<User>>.Success(users));
        A.CallTo(() => _taskRepository.GetTasks()).Returns(Result<IEnumerable<DomainTask>>.Success(new List<DomainTask> { task }));
        
        _taskEngine = new TaskEngine(_taskRepository, _userRepository, _logger, _userRandomizer);
        _taskEngine.InitializeAsync().Wait();

        // Act
        _taskEngine.RotateTask(task);

        // Assert
        task.AssignedUser.Name.Should().NotBe("User1");
    }

    [Fact]
    public void RotateTask_Should_SetStateAsWaiting_When_NoUsersAvailable()
    {
        // Arrange
        var users = new List<User> { new User("User1") };
        var task = Domain.Task.NewTask("Description 1");
        task.AssignedUser = new User("User1"); 
        task.AssignedUsersHistory = new List<User> { task.AssignedUser };

        A.CallTo(() => _userRepository.GetUsers()).Returns(Result<IEnumerable<User>>.Success(users));
        A.CallTo(() => _taskRepository.GetTasks()).Returns(Result<IEnumerable<DomainTask>>.Success(new List<DomainTask> { task }));

        _taskEngine = new TaskEngine(_taskRepository, _userRepository, _logger, _userRandomizer);

        // Act
        _taskEngine.RotateTask(task);

        // Assert
        task.State.Should().Be(TaskState.Waiting);
        task.AssignedUser.Should().BeOfType(NullUser.Instance.GetType());
    }
}