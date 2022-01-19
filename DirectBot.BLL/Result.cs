using DirectBot.Core.Interfaces;

namespace DirectBot.BLL;

public class Result<T> : IResult<T>
{
    public bool Succeeded { get; }
    public string? ErrorMessage { get; }
    public T? Value { get; }

    private Result(bool succeeded, string? errorMessage, T? value)
    {
        Succeeded = succeeded;
        ErrorMessage = errorMessage;
        Value = value;
    }

    public static Result<T> Ok(T value)
    {
        return new Result<T>(true, null, value);
    }
    
    public static Result<T> Fail(string message, T? value = default)
    {
        return new Result<T>(false, message, value);
    }
}