namespace Cnvs.Demo.TaskManagement.Domain;

public class NullTask : Task
{
    public static NullTask Instance { get; } = new();

    private NullTask() : base(Guid.Empty, "Null Task")
    {
    }
}