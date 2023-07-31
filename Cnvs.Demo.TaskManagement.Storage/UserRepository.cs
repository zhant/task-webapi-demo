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

    public Task<Result<User>> GetRandomUserAsync()
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
            _logger.LogInformation("Adding new user: {UserName}", user.Name);
            // TODO: Implement this Writing to the database
            // _logger.LogInformation("Successfully added user: {userName}", user.Name);
            return await Task.FromResult(Result<User>.Success(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding user: {UserName}", user.Name);
            throw;
        }
    }

    public Task<Result<User>> GetUserAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<User>> GetUserByNameAsync(string userName)
    {
        try
        {
            _logger.LogInformation("Searching for user: {UserName}", userName);
            // TODO: Implement this Reading from the database
            var nullUser = NullUser.Instance;
            // _logger.LogInformation("Found user: {userName}", userName);
            return await Task.FromResult(Result<User>.Success(nullUser));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving user: {UserName}", userName);
            throw;
        }
    }

    public Task<Result<IEnumerable<User>>> GetUsersAsync()
    {
        try
        {
            _logger.LogInformation("Getting all users");
            
            // TODO: Implement this Reading from the database
            var users = Enumerable.Empty<User>();

            _logger.LogInformation("Successfully retrieved all users");
            return Task.FromResult(Result<IEnumerable<User>>.Success(users));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all users");
            throw;
        }
    }

    public Task<Result<string>> DeleteUserAsync(string userName)
    {
        try
        {
            _logger.LogInformation("Deleting user: {UserName}", userName);
            throw new NotImplementedException();
            // _logger.LogInformation("Successfully deleted user: {userName}", userName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting user: {UserName}", userName);
            throw;
        }
    }
}