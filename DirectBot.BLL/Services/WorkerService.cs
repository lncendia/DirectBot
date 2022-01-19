using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;
using DirectBot.Core.Repositories;
using DirectBot.Core.Services;

namespace DirectBot.BLL.Services;

public class WorkerService : IWorkerService
{
    private readonly IWorkRepository _workRepository;

    public WorkerService(IWorkRepository workRepository)
    {
        _workRepository = workRepository;
    }

    public string StartWork(WorkDTO work)
    {
        return Hangfire.BackgroundJob.Enqueue(() => Console.WriteLine("started"));
    }

    public string ScheduleWork(WorkDTO work, DateTimeOffset offset)
    {
        return Hangfire.BackgroundJob.Schedule(() => Console.WriteLine("started"), offset);
    }

    public IOperationResult CancelWorkAsync(WorkDTO work)
    {
        if (string.IsNullOrEmpty(work.JobId)) throw new NullReferenceException("JobId in null");
        var result = Hangfire.BackgroundJob.Delete(work.JobId);
        return !result
            ? OperationResult.Fail("Failed to stop work")
            : OperationResult.Ok();
    }
}