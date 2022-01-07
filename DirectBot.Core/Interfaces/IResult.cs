namespace DirectBot.Core.Interfaces;

public interface IResult<out T>
{
    public bool Succeeded { get; }
    public string? ErrorMessage { get; }
    public T? Value { get; }
}