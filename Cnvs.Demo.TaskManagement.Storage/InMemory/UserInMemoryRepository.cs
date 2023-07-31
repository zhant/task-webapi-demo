using System.Collections.Concurrent;
using Cnvs.Demo.TaskManagement.Domain;
using DomainTask = Cnvs.Demo.TaskManagement.Domain.Task;
using Task = System.Threading.Tasks.Task;

namespace Cnvs.Demo.TaskManagement.Storage.InMemory;

public class UserInMemoryRepository : IUserRepository
{
    private readonly ConcurrentDictionary<string, User> _users = new();

    public async Task<Result<User>> AddUser(User user)
    {
        var added = _users.TryAdd(user.Name, user);
        return !added 
            ? Result<User>.Failure("User already exists", NullUser.Instance) 
            : Result<User>.Success(user);
    }

    public async Task<Result<User>> GetUserAsync(Guid id)
    {
        var value = _users.ToArray().FirstOrDefault(x => x.Value.Id == id.ToString()).Value ?? NullUser.Instance;
        return await Task.FromResult(Result<User>.Success(value));
    }

    public async Task<Result<User>> GetUserByNameAsync(string userName)
    {
        _users.TryGetValue(userName, out var user);
        user ??= NullUser.Instance;
        return Result<User>.Success(user);
    }

    public Result<IEnumerable<User>> GetUsers()
    {
        var users = _users.Values.ToList();
        return Result<IEnumerable<User>>.Success(users);
    }

    public async Task<Result<string>> DeleteUserAsync(string userName)
    {
        var removed = _users.TryRemove(userName, out var _);
        
        return removed 
            ? Result<string>.Success(userName) 
            : Result<string>.Failure("User not found", userName);
    }
}