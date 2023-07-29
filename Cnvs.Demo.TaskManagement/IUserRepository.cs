using Cnvs.Demo.TaskManagement.Domain;

namespace Cnvs.Demo.TaskManagement;

public interface IUserRepository
{
    Task<Result<User>> AddUser(User user);
    Task<Result<User>> GetUserAsync(string userName);
    Result<IEnumerable<User>> GetUsers();
    Task<Result<string>> DeleteUserAsync(string userName);
}