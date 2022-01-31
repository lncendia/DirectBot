using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IWorkNotifier
{
     Task<IOperationResult> NotifyStartAsync(WorkDto workDto);
     Task<IOperationResult> NotifyEndAsync(WorkDto workDto);
}