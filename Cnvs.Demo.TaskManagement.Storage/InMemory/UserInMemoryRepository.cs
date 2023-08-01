using System.Collections.Concurrent;
using Cnvs.Demo.TaskManagement.Domain;
using DomainTask = Cnvs.Demo.TaskManagement.Domain.Task;
using Task = System.Threading.Tasks.Task;

namespace Cnvs.Demo.TaskManagement.Storage.InMemory;

public class UserInMemoryRepository : IUserRepository
{
    private readonly ConcurrentDictionary<string, User> _users = new();

    public Task<Result<User>> AddUser(User user)
    {
        var added = _users.TryAdd(user.Name, user);
        return !added 
            ? Task.FromResult(Result<User>.Failure("User already exists", user)) 
            : Task.FromResult(Result<User>.Success(user));
    }

    public Task<Result<User>> GetUserAsync(Guid id)
    {
        var user = _users.ToArray().FirstOrDefault(x => x.Value.Id == id.ToString()).Value ?? NullUser.Instance;
        return user is not NullUser
            ? Task.FromResult(Result<User>.Success(user)) 
            : Task.FromResult(Result<User>.Failure($"User {id} not found", user));
    }

    public Task<Result<User>> GetUserByNameAsync(string userName)
    {
        var value = _users.TryGetValue(userName, out var user);
        user ??= NullUser.Instance;
        return value
            ? Task.FromResult(Result<User>.Success(user)) 
            : Task.FromResult(Result<User>.Failure($"User {userName} not found", user));
    }
 
    public Task<Result<IEnumerable<User>>> GetUsersAsync()
    {
        var users = _users.Values.ToList();
        return Task.FromResult(Result<IEnumerable<User>>.Success(users));
    }

    public Task<Result<string>> DeleteUserAsync(string userName)
    {
        var removed = _users.TryRemove(userName, out var _);
        
        return removed 
            ? Task.FromResult(Result<string>.Success(userName)) 
            : Task.FromResult(Result<string>.Failure("Failed to delete user", userName));
    }
}