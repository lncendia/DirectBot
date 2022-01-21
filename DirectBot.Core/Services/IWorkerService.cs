using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IWorkerService
{
    public string StartWork(WorkDto work);
    public string ScheduleWork(WorkDto work, DateTimeOffset dateTimeOffset);
    public IOperationResult CancelWorkAsync(WorkDto work);
}