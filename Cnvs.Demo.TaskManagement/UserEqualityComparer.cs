using Cnvs.Demo.TaskManagement.Domain;

namespace Cnvs.Demo.TaskManagement;

internal class UserEqualityComparer : IEqualityComparer<User>
{
    public bool Equals(User? x, User? y)
    {
        return x?.Name == y?.Name;
    }

    public int GetHashCode(User obj)
    {
        return obj.Name.GetHashCode();
    }
}