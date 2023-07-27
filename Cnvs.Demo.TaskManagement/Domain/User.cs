namespace Cnvs.Demo.TaskManagement.Domain;

public class User
{
    /// <summary>
    /// Name is a unique identifier for the user.
    /// </summary>
    public string Name { get; set; } = null!;
    
    public IEnumerable<Task> Tasks { get; set; } = new List<Task>();

    public User()
    {
    }

    public User(string name)
    {
        Name = name;
    }
}

public class NullUser : User
{
    public static NullUser Instance { get; } = new NullUser();

    private NullUser() : base("Null User")
    {
    }
}