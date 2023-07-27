namespace Cnvs.Demo.TaskManagement;

public class Result<T>
{
    private Result(T value)
    {
        Value = value;
    }

    public T Value { get; }
    public bool IsSuccess { get; private init; } = true;
    public bool IsFailure => !IsSuccess;
    public string ErrorMessage { get; private init; } = string.Empty;

    public static Result<T> Failure(string errorMessage, T value)
    {
        return new Result<T>(value)
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
    
    // add success
    public static Result<T> Success(T value)
    {
        return new Result<T>(value);
    }
}