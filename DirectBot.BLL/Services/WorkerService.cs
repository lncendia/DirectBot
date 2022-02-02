using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;
using DirectBot.Core.Services;
using Hangfire;

namespace DirectBot.BLL.Services;

public class WorkerService : IWorkerService
{
    private readonly IWorkService _workService;
    private readonly IBackgroundJobService _backgroundJobService;

    public WorkerService(IBackgroundJobService backgroundJobService, IWorkService workService)
    {
        _backgroundJobService = backgroundJobService;
        _workService = workService;
    }

    public Task<IOperationResult> StartWorkAsync(WorkDto work)
    {
        work.JobId = BackgroundJob.Enqueue(() => _backgroundJobService.RunAsync(work.Id, CancellationToken.None));
        BackgroundJob.ContinueJobWith(work.JobId, () => _backgroundJobService.SaveAfterContinuedAsync(work.Id),
            JobContinuationOptions.OnAnyFinishedState);
        work.StartTime = DateTime.Now;
        return _workService.UpdateAsync(work);
    }

    public Task<IOperationResult> ScheduleWorkAsync(WorkDto work, DateTimeOffset offset)
    {
        work.JobId =
            BackgroundJob.Schedule(() => _backgroundJobService.RunAsync(work.Id, CancellationToken.None), offset);
        BackgroundJob.ContinueJobWith(work.JobId, () => _backgroundJobService.SaveAfterContinuedAsync(work.Id),
            JobContinuationOptions.OnAnyFinishedState);
        work.StartTime = offset.DateTime;
        return _workService.UpdateAsync(work);
    }

    public Task<IOperationResult> CancelWorkAsync(WorkDto work)
    {
        if (string.IsNullOrEmpty(work.JobId)) throw new NullReferenceException("JobId in null");
        var result = BackgroundJob.Delete(work.JobId);
        if (!result) OperationResult.Fail("Failed to stop work");
        work.IsCompleted = true;
        work.IsCanceled = true;
        return _workService.UpdateAsync(work);
    }
}