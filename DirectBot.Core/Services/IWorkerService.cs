using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IWorkerService
{
    public string StartWork(WorkDTO work);
    public string ScheduleWork(WorkDTO work, DateTimeOffset dateTimeOffset);
    public IOperationResult CancelWorkAsync(WorkDTO work);
}