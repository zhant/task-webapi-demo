using System.Collections.Concurrent;
using System.Collections.Immutable;
using Cnvs.Demo.TaskManagement.Domain;
using DomainTask = Cnvs.Demo.TaskManagement.Domain.Task;

namespace Cnvs.Demo.TaskManagement.Storage;

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

    public async Task<Result<User>> GetUserAsync(string userName)
    {
        _users.TryGetValue(userName, out var user);
        user ??= NullUser.Instance;

        return user is NullUser 
            ? Result<User>.Failure("User not found", user) 
            : Result<User>.Success(user);
    }

    public Result<IEnumerable<User>> GetUsers()
    {
        var users = _users.Values.ToList();

        return users.Any() 
            ? Result<IEnumerable<User>>.Success(users) 
            : Result<IEnumerable<User>>.Failure("No users found", ImmutableList<User>.Empty);
    }

    public async Task<Result<string>> DeleteUserAsync(string userName)
    {
        var removed = _users.TryRemove(userName, out var _);
        
        return removed 
            ? Result<string>.Success(userName) 
            : Result<string>.Failure("User not found", userName);
    }
}