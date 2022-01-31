namespace DirectBot.Core.Interfaces;

public interface IOperationResult
{
    bool Succeeded { get; }
    string? ErrorMessage { get; }
}