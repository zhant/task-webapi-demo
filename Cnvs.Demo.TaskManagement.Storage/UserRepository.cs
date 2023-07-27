using Cnvs.Demo.TaskManagement.Domain;

namespace Cnvs.Demo.TaskManagement.Storage;

public class UserRepository : IUserRepository
{
    public async Task<Result<User>> GetRandomUserAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Result<User>> AddUser(User user)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<User>> GetUserAsync(string userName)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<IEnumerable<User>>> GetUsersAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Result<string>> DeleteUserAsync(string userName)
    {
        throw new NotImplementedException();
    }
}