namespace Cnvs.Demo.TaskManagement.Domain;

public class NullUser : User
{
    public static NullUser Instance { get; } = new();

    private NullUser() : base("Null User", Guid.Empty)
    {
    }
}