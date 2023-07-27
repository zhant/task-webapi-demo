namespace Cnvs.Demo.TaskManagement.Dto;

public class User
{
    public string Name { get; set; } = string.Empty;
    public IEnumerable<Task> Tasks { get; set; } = new List<Task>();

    public User()
    {
    }

    public User(string name)
    {
        Name = name;
    }
}
