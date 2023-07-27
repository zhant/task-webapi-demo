using Cnvs.Demo.TaskManagement.Domain;

namespace Cnvs.Demo.TaskManagement;

public interface IUserRandomizer
{
    User GetRandomUser(IEnumerable<User> users);
}