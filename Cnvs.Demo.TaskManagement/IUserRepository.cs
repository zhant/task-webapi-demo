﻿using Cnvs.Demo.TaskManagement.Domain;

namespace Cnvs.Demo.TaskManagement;

public interface IUserRepository
{
    Task<Result<User>> GetRandomUserAsync();
    Task<Result<User>> AddUser(User user);
    Task<Result<User>> GetUserAsync(string userName);
    Task<Result<IEnumerable<User>>> GetUsersAsync();
    Task<Result<string>> DeleteUserAsync(string userName);
}