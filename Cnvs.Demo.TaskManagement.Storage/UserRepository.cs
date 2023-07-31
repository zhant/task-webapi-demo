using System.Collections.Immutable;
using Cnvs.Demo.TaskManagement.Domain;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;

namespace Cnvs.Demo.TaskManagement.Storage;

public class UserRepository : IUserRepository
{
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(ILogger<UserRepository> logger)
    {
        _logger = logger;
    }

    public async Task<Result<User>> GetRandomUserAsync()
    {
        try
        {
            _logger.LogInformation("Attempting to retrieve random user");
            throw new NotImplementedException();
            // _logger.LogInformation("Successfully retrieved random user: {userId}", user.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting the random user");
            throw;
        }
    }

    public async Task<Result<User>> AddUser(User user)
    {
        try
        {
            _logger.LogInformation("Adding new user: {userName}", user.Name);
            // TODO: Implement this Writing to the database
            // _logger.LogInformation("Successfully added user: {userName}", user.Name);
            return await Task.FromResult(Result<User>.Success(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding user: {userName}", user.Name);
            throw;
        }
    }

    public async Task<Result<User>> GetUserAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<User>> GetUserByNameAsync(string userName)
    {
        try
        {
            _logger.LogInformation("Searching for user: {userName}", userName);
            // TODO: Implement this Reading from the database
            var nullUser = NullUser.Instance;
            // _logger.LogInformation("Found user: {userName}", userName);
            return await System.Threading.Tasks.Task.FromResult(Result<User>.Success(nullUser));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving user: {userName}", userName);
            throw;
        }
    }

    public Result<IEnumerable<User>> GetUsers()
    {
        try
        {
            _logger.LogInformation("Getting all users");
            
            // TODO: Implement this Reading from the database
            var users = ImmutableList<User>.Empty;

            _logger.LogInformation("Successfully retrieved all users");
            return Result<IEnumerable<User>>.Success(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all users");
            throw;
        }
    }

    public async Task<Result<string>> DeleteUserAsync(string userName)
    {
        try
        {
            _logger.LogInformation("Deleting user: {userName}", userName);
            throw new NotImplementedException();
            // _logger.LogInformation("Successfully deleted user: {userName}", userName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting user: {userName}", userName);
            throw;
        }
    }
}