using Cnvs.Demo.TaskManagement.Domain;

namespace Cnvs.Demo.TaskManagement;

public interface IUserRepository
{
    Task<Result<User>> AddUser(User user);
    Task<Result<User>> GetUserAsync(Guid id);
    Task<Result<User>> GetUserByNameAsync(string userName);
    Result<IEnumerable<User>> GetUsers();
    Task<Result<string>> DeleteUserAsync(string userName);
}