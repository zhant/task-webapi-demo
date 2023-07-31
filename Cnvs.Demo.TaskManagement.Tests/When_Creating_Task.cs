using Cnvs.Demo.TaskManagement.Configuration;
using Cnvs.Demo.TaskManagement.Domain;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Task = System.Threading.Tasks.Task;

namespace Cnvs.Demo.TaskManagement.Tests;

public class When_Creating_Task
{
    private readonly TaskEngine _taskEngine;
    private readonly ITaskRepository _fakeTaskRepo;
    private readonly IUserRepository _fakeUserRepo;
    private readonly IUserRandomizer _userRandomizer;
    private const string TaskDescription = "Test Task";

    public When_Creating_Task()
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
    public async Task CreateTaskAsync_ShouldReturnFailure_WhenAddTaskFails()
    {
        // Arrange

        A.CallTo(() => _fakeTaskRepo.AddTaskAsync(A<Domain.Task>._))
            .Returns(Result<Domain.Task>.Failure("Storage error", NullTask.Instance));
        var testUser = User.Create("TestUser");
        A.CallTo(() => _fakeUserRepo.GetUsersAsync())
            .Returns(Result<IEnumerable<User>>.Success(new[] { testUser }));

        // Act
        var result = await _taskEngine.CreateTaskAsync(TaskDescription);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Storage error");
    }

    [Fact]
    public async Task CreateTaskAsync_ShouldReturnSuccess_WhenAddTaskSucceeds()
    {
        // Arrange
        var testTask = Domain.Task.NewTask(TaskDescription);

        var testUser = User.Create("TestUser");
        A.CallTo(() => _fakeUserRepo.GetUsersAsync())
            .Returns(Result<IEnumerable<User>>.Success(new[] { testUser }));
        A.CallTo(() => _fakeTaskRepo.AddTaskAsync(A<Domain.Task>._))
            .Returns(Result<Domain.Task>.Success(testTask));
        A.CallTo(() => _userRandomizer.GetRandomUser(A<IEnumerable<User>>._))
            .Returns(User.Create("TestUser"));

        // Act
        var result = await _taskEngine.CreateTaskAsync(TaskDescription);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(testTask);
    }

    [Fact]
    public async Task CreateTaskAsync_AssignedUserShouldBeNull_WhenGetRandomUserReturnsNullUserInstance()
    {
        // Arrange
        A.CallTo(() => _fakeUserRepo.GetUsersAsync())
            .Returns(Result<IEnumerable<User>>.Success(Enumerable.Empty<User>()));
        A.CallTo(() => _userRandomizer.GetRandomUser(A<IEnumerable<User>>._))
            .Returns(NullUser.Instance);

        A.CallTo(() => _fakeTaskRepo.AddTaskAsync(A<Domain.Task>._))
            .ReturnsLazily((Domain.Task t) => Result<Domain.Task>.Success(t));

        // Act
        var result = await _taskEngine.CreateTaskAsync(TaskDescription);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.AssignedUser.Should().BeOfType<NullUser>();
    }

    [Fact]
    public async Task CreateTaskAsync_AssignedUserShouldNotBeNull_WhenGetRandomUserReturnsUserInstance()
    {
        // Arrange
        var testUser = User.Create("TestUser");
        A.CallTo(() => _fakeUserRepo.GetUsersAsync())
            .Returns(Result<IEnumerable<User>>.Success(new[] { testUser }));
        A.CallTo(() => _userRandomizer.GetRandomUser(A<IEnumerable<User>>._))
            .Returns(testUser);
        
        A.CallTo(() => _fakeTaskRepo.AddTaskAsync(A<Domain.Task>._))
            .ReturnsLazily((Domain.Task t) => Result<Domain.Task>.Success(t));

        // Act
        var result = await _taskEngine.CreateTaskAsync(TaskDescription);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.AssignedUser.Should().BeEquivalentTo(testUser);
    }
    
    [Fact]
    public async Task CreateTaskAsync_ShouldThrowArgumentException_WhenTaskDescriptionIsEmpty()
    {
        // Arrange
        const string emptyDescription = "";

        // Act
        Func<Task> act = async () => await _taskEngine.CreateTaskAsync(emptyDescription);

        // Assert
        await act.Should().ThrowExactlyAsync<ArgumentException>()
            .WithMessage("Description cannot be empty");
    }

    [Fact]
    public async Task CreateTaskAsync_ShouldAddTaskToRepository_WhenTaskIsValid()
    {
        // Arrange
        var testUser = User.Create("TestUser");
        A.CallTo(() => _fakeUserRepo.GetUsersAsync())
            .Returns(Result<IEnumerable<User>>.Success(new[] { testUser }));
        const string validDescription = "Valid description";

        // Act
        await _taskEngine.CreateTaskAsync(validDescription);

        // Assert
        A.CallTo(() => _fakeTaskRepo.AddTaskAsync(A<Domain.Task>._)).MustHaveHappened();
    }
}