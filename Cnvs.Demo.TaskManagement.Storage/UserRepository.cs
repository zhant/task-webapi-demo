using Cnvs.Demo.TaskManagement.Domain;
using Microsoft.Extensions.Logging;

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
            throw new NotImplementedException();
            // _logger.LogInformation("Successfully added user: {userName}", user.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding user: {userName}", user.Name);
            throw;
        }
    }

    public async Task<Result<User>> GetUserAsync(string userName)
    {
        try
        {
            _logger.LogInformation("Searching for user: {userName}", userName);
            throw new NotImplementedException();
            // _logger.LogInformation("Found user: {userName}", userName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving user: {userName}", userName);
            throw;
        }
    }

    public async Task<Result<IEnumerable<User>>> GetUsersAsync()
    {
        try
        {
            _logger.LogInformation("Getting all users");
            throw new NotImplementedException();
            // _logger.LogInformation("Successfully retrieved all users.");
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