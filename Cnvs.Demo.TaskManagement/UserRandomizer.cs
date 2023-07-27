using Cnvs.Demo.TaskManagement.Domain;

namespace Cnvs.Demo.TaskManagement;

public class UserRandomizer : IUserRandomizer
{
    private static readonly Random Random = new();

    public User GetRandomUser(IEnumerable<User> users)
    {
        var enumerable = users.ToList();
        if (!enumerable.Any())
        {
            return NullUser.Instance;
        }

        var index = Random.Next(enumerable.Count);
        return enumerable[index];
    }
}