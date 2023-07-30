using Cnvs.Demo.TaskManagement.Domain;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;

namespace Cnvs.Demo.TaskManagement.Tests;

public class When_Creating_User
{
    private const string UserName = "TestUser";
    private readonly User _testUser = new("TestUser");
    
    [Fact]
    public async Task CreateUserAsync_ShouldReturnFailure_WhenGetUserAsyncFails()
    {
        // Arrange
        var fakeTaskRepo = A.Fake<ITaskRepository>();
        var fakeUserRepo = A.Fake<IUserRepository>();
        var fakeLogger = A.Fake<ILogger<TaskEngine>>();
        var userRandomizer = A.Fake<IUserRandomizer>();

        A.CallTo(() => fakeUserRepo.GetUserAsync(UserName))
            .Returns(Result<User>.Failure("Database error", NullUser.Instance));

        var taskEngine = new TaskEngine(fakeTaskRepo, fakeUserRepo, fakeLogger, userRandomizer);

        // Act
        var result = await taskEngine.CreateUserAsync(UserName);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be($"Failed to get user {UserName}: Database error");
    }

    [Fact]
    public async Task CreateUserAsync_ShouldReturnFailure_WhenUserAlreadyExists()
    {
        // Arrange
        var fakeTaskRepo = A.Fake<ITaskRepository>();
        var fakeUserRepo = A.Fake<IUserRepository>();
        var fakeLogger = A.Fake<ILogger<TaskEngine>>();
        var userRandomizer = A.Fake<IUserRandomizer>();

        A.CallTo(() => fakeUserRepo.GetUserAsync(UserName))
            .Returns(Result<User>.Success(_testUser));

        var taskEngine = new TaskEngine(fakeTaskRepo, fakeUserRepo, fakeLogger, userRandomizer);

        // Act
        var result = await taskEngine.CreateUserAsync(UserName);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be($"User with name {UserName} already exists");
    }

    [Fact]
    public async Task CreateUserAsync_ShouldReturnFailure_WhenAddUserFails()
    {
        // Arrange
        var fakeTaskRepo = A.Fake<ITaskRepository>();
        var fakeUserRepo = A.Fake<IUserRepository>();
        var fakeLogger = A.Fake<ILogger<TaskEngine>>();
        var userRandomizer = A.Fake<IUserRandomizer>();

        A.CallTo(() => fakeUserRepo.GetUserAsync(UserName))
            .Returns(Result<User>.Success(NullUser.Instance));
        
        var failure = Result<User>.Failure($"Failed to add user to repository: {UserName}", NullUser.Instance);
        A.CallTo(() => fakeUserRepo.AddUser(A<User>._))
            .Returns(failure);

        var taskEngine = new TaskEngine(fakeTaskRepo, fakeUserRepo, fakeLogger, userRandomizer);

        // Act
        var result = await taskEngine.CreateUserAsync(UserName);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().StartWith($"Failed to add user to repository: {UserName}");
    }

    [Fact]
    public async Task CreateUserAsync_ShouldReturnSuccess_WhenUserIsAddedSuccessfully()
    {
        // Arrange
        var fakeTaskRepo = A.Fake<ITaskRepository>();
        var fakeUserRepo = A.Fake<IUserRepository>();
        var fakeLogger = A.Fake<ILogger<TaskEngine>>();
        var userRandomizer = A.Fake<IUserRandomizer>();

        A.CallTo(() => fakeUserRepo.GetUserAsync(UserName))
            .Returns(Result<User>.Success(NullUser.Instance));
        A.CallTo(() => fakeUserRepo.AddUser(A<User>.That.Matches(u => u.Name == UserName)))
            .Returns(Result<User>.Success(_testUser));

        var taskEngine = new TaskEngine(fakeTaskRepo, fakeUserRepo, fakeLogger, userRandomizer);

        // Act
        var result = await taskEngine.CreateUserAsync(UserName);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(_testUser);
    }
}