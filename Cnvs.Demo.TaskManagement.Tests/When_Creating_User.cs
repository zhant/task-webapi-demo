using Cnvs.Demo.TaskManagement.Configuration;
using Cnvs.Demo.TaskManagement.Domain;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Task = System.Threading.Tasks.Task;

namespace Cnvs.Demo.TaskManagement.Tests;

public class When_Creating_User
{
    private const string UserName = "TestUser";
    private readonly User _testUser = User.Create(UserName);
    private readonly TaskEngine _taskEngine;
    private readonly IUserRepository _fakeUserRepo;

    public When_Creating_User()
    {
        var fakeTaskRepo = A.Fake<ITaskRepository>();
        _fakeUserRepo = A.Fake<IUserRepository>();
        var fakeLogger = A.Fake<ILogger<TaskEngine>>();
        var userRandomizer = A.Fake<IUserRandomizer>();
        var options = A.Fake<IOptionsSnapshot<TaskEngineOptions>>();
        _taskEngine = new TaskEngine(fakeTaskRepo,
            _fakeUserRepo,
            fakeLogger,
            userRandomizer,
            options);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldReturnFailure_WhenGetUserAsyncFails()
    {
        // Arrange
        A.CallTo(() => _fakeUserRepo.GetUserByNameAsync(UserName))
            .Returns(Result<User>.Failure("Storage error", NullUser.Instance));

        // Act
        var result = await _taskEngine.CreateUserAsync(User.Create(UserName));

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be($"Failed to get user {UserName}: Storage error");
    }

    [Fact]
    public async Task CreateUserAsync_ShouldReturnFailure_WhenUserAlreadyExists()
    {
        // Arrange
        A.CallTo(() => _fakeUserRepo.GetUserByNameAsync(UserName))
            .Returns(Result<User>.Success(_testUser));

        // Act
        var result = await _taskEngine.CreateUserAsync(User.Create(UserName));

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be($"User with name {UserName} already exists");
    }

    [Fact]
    public async Task CreateUserAsync_ShouldReturnFailure_WhenAddUserFails()
    {
        // Arrange
        A.CallTo(() => _fakeUserRepo.GetUserByNameAsync(UserName))
            .Returns(Result<User>.Success(NullUser.Instance));
        
        var failure = Result<User>.Failure($"Failed to add user to repository: {UserName}", NullUser.Instance);
        A.CallTo(() => _fakeUserRepo.AddUser(A<User>._))
            .Returns(failure);

        // Act
        var result = await _taskEngine.CreateUserAsync(User.Create(UserName));

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().StartWith($"Failed to add user to repository: {UserName}");
    }

    [Fact]
    public async Task CreateUserAsync_ShouldReturnSuccess_WhenUserIsAddedSuccessfully()
    {
        // Arrange
        A.CallTo(() => _fakeUserRepo.GetUserByNameAsync(UserName))
            .Returns(Result<User>.Success(NullUser.Instance));
        A.CallTo(() => _fakeUserRepo.AddUser(A<User>.That.Matches(u => u.Name == UserName)))
            .Returns(Result<User>.Success(_testUser));

        // Act
        var result = await _taskEngine.CreateUserAsync(User.Create(UserName));

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(_testUser);
    }
}