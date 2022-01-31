using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IWorkerService
{
     Task<IOperationResult> StartWorkAsync(WorkDto work);
     Task<IOperationResult> ScheduleWorkAsync(WorkDto work, DateTimeOffset dateTimeOffset);
     Task<IOperationResult> CancelWorkAsync(WorkDto work);
}