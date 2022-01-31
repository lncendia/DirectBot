namespace DirectBot.Core.Interfaces;

public interface IResult<out T>
{
    bool Succeeded { get; }
    string? ErrorMessage { get; }
    T? Value { get; }
}