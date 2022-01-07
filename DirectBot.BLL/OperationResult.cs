using DirectBot.Core.Interfaces;

namespace DirectBot.BLL;

public class OperationResult : IOperationResult
{
    public bool Succeeded { get; }
    public string? ErrorMessage { get; }

    private OperationResult(bool succeeded, string? errorMessage)
    {
        Succeeded = succeeded;
        ErrorMessage = errorMessage;
    }

    public static OperationResult Ok()
    {
        return new OperationResult(true, null);
    }

    public static OperationResult Fail(string message)
    {
        return new OperationResult(false, message);
    }
}