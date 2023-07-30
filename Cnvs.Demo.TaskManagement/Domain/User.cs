using System.Diagnostics.CodeAnalysis;

namespace Cnvs.Demo.TaskManagement.Domain;

public class User
{
    /// <summary>
    /// Name is a unique identifier for the user.
    /// </summary>
    public string Name { get; set; } = null!;
    
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }
    public IEnumerable<Task> Tasks { get; set; } = new List<Task>();

    public User()
    {
    }

    private protected User(string name, Guid id)
    {
        Name = name;
        Id = id.ToString();
    }

    private User(string name)
    {
        Name = name;
    }

    public static User Create(string name)
    {
        return new User(name);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }
        
        return Name == ((User)obj).Name && Id == ((User)obj).Id;
    }

    [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
    public override int GetHashCode()
    {
        return Name.GetHashCode() ^ Id.GetHashCode();
    }
}