using Cnvs.Demo.TaskManagement.Domain;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;

namespace Cnvs.Demo.TaskManagement.Tests;

public class When_Creating_Task
{
    private const string TaskDescription = "Test Task";

    [Fact]
    public async Task CreateTaskAsync_ShouldReturnFailure_WhenAddTaskFails()
    {
        // Arrange
        var fakeTaskRepo = A.Fake<ITaskRepository>();
        var fakeUserRepo = A.Fake<IUserRepository>();
        var fakeLogger = A.Fake<ILogger<TaskEngine>>();
        var userRandomizer = A.Fake<IUserRandomizer>();
        var taskEngine = new TaskEngine(fakeTaskRepo, fakeUserRepo, fakeLogger, userRandomizer);
        
        A.CallTo(() => fakeTaskRepo.AddTask(A<Domain.Task>._))
            .Returns(Result<Domain.Task>.Failure("Database error", NullTask.Instance));
        
        // Act
        var result = await taskEngine.CreateTaskAsync(TaskDescription);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Database error");
    }

    [Fact]
    public async Task CreateTaskAsync_ShouldReturnSuccess_WhenAddTaskSucceeds()
    {
        // Arrange
        var fakeTaskRepo = A.Fake<ITaskRepository>();
        var fakeUserRepo = A.Fake<IUserRepository>();
        var fakeLogger = A.Fake<ILogger<TaskEngine>>();
        var userRandomizer = A.Fake<IUserRandomizer>();
        var testTask = Domain.Task.NewTask(TaskDescription);

        A.CallTo(() => fakeTaskRepo.AddTask(A<Domain.Task>._))
            .Returns(Result<Domain.Task>.Success(testTask));
        A.CallTo(() => userRandomizer.GetRandomUser(A<IEnumerable<User>>._))
            .Returns(User.Create("TestUser"));

        var taskEngine = new TaskEngine(fakeTaskRepo, fakeUserRepo, fakeLogger, userRandomizer);

        // Act
        var result = await taskEngine.CreateTaskAsync(TaskDescription);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(testTask);
    }

    [Fact]
    public async Task CreateTaskAsync_AssignedUserShouldBeNull_WhenGetRandomUserReturnsNullUserInstance()
    {
        // Arrange
        var fakeTaskRepo = A.Fake<ITaskRepository>();
        var fakeUserRepo = A.Fake<IUserRepository>();
        var fakeLogger = A.Fake<ILogger<TaskEngine>>();
        var userRandomizer = A.Fake<IUserRandomizer>();
        var taskEngine = new TaskEngine(fakeTaskRepo, fakeUserRepo, fakeLogger, userRandomizer);

        A.CallTo(() => userRandomizer.GetRandomUser(A<IEnumerable<User>>._))
            .Returns(NullUser.Instance);

        A.CallTo(() => fakeTaskRepo.AddTask(A<Domain.Task>._))
            .ReturnsLazily((Domain.Task t) => Result<Domain.Task>.Success(t));

        // Act
        var result = await taskEngine.CreateTaskAsync(TaskDescription);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.AssignedUser.Should().BeOfType<NullUser>();
    }

    [Fact]
    public async Task CreateTaskAsync_AssignedUserShouldNotBeNull_WhenGetRandomUserReturnsUserInstance()
    {
        // Arrange
        var fakeTaskRepo = A.Fake<ITaskRepository>();
        var fakeUserRepo = A.Fake<IUserRepository>();
        var fakeLogger = A.Fake<ILogger<TaskEngine>>();
        var userRandomizer = A.Fake<IUserRandomizer>();
        var taskEngine = new TaskEngine(fakeTaskRepo, fakeUserRepo, fakeLogger, userRandomizer);

        var testUser = User.Create("TestUser");
        A.CallTo(() => userRandomizer.GetRandomUser(A<IEnumerable<User>>._))
            .Returns(testUser);
        
        A.CallTo(() => fakeTaskRepo.AddTask(A<Domain.Task>._))
            .ReturnsLazily((Domain.Task t) => Result<Domain.Task>.Success(t));

        // Act
        var result = await taskEngine.CreateTaskAsync(TaskDescription);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.AssignedUser.Should().BeEquivalentTo(testUser);
    }
    
    [Fact]
    public async Task CreateTaskAsync_ShouldThrowArgumentException_WhenTaskDescriptionIsEmpty()
    {
        // Arrange
        var fakeTaskRepo = A.Fake<ITaskRepository>();
        var fakeUserRepo = A.Fake<IUserRepository>();
        var fakeLogger = A.Fake<ILogger<TaskEngine>>();
        var userRandomizer = A.Fake<IUserRandomizer>();
        var taskEngine = new TaskEngine(fakeTaskRepo, fakeUserRepo, fakeLogger, userRandomizer);
        const string emptyDescription = "";

        // Act
        Func<Task> act = async () => await taskEngine.CreateTaskAsync(emptyDescription);

        // Assert
        await act.Should().ThrowExactlyAsync<ArgumentException>()
            .WithMessage("Description cannot be empty");
    }

    [Fact]
    public async Task CreateTaskAsync_ShouldAddTaskToRepository_WhenTaskIsValid()
    {
        // Arrange
        var fakeTaskRepo = A.Fake<ITaskRepository>();
        var fakeUserRepo = A.Fake<IUserRepository>();
        var fakeLogger = A.Fake<ILogger<TaskEngine>>();
        var userRandomizer = A.Fake<IUserRandomizer>();
        var taskEngine = new TaskEngine(fakeTaskRepo, fakeUserRepo, fakeLogger, userRandomizer);
        const string validDescription = "Valid description";

        // Act
        await taskEngine.CreateTaskAsync(validDescription);

        // Assert
        A.CallTo(() => fakeTaskRepo.AddTask(A<Domain.Task>._)).MustHaveHappened();
    }
}