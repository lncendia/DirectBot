namespace DirectBot.Core.Interfaces;

public interface IOperationResult
{
    public bool Succeeded { get; }
    public string? ErrorMessage { get; }
}