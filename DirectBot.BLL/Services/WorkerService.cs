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
        if (!work.Instagrams.Any())
            return Task.FromResult((IOperationResult) OperationResult.Fail("Работа не содержит инстаграм(ы)"));
        if (work.IsCompleted)
            return Task.FromResult((IOperationResult) OperationResult.Fail("Работа не может быть запущена"));


        work.JobId =
            BackgroundJob.Enqueue(() => _backgroundJobService.ProcessingAsync(work.Id, CancellationToken.None));

        BackgroundJob.ContinueJobWith(work.JobId, () => _backgroundJobService.SaveAfterContinuedAsync(work.Id),
            JobContinuationOptions.OnAnyFinishedState);


        work.StartTime = DateTime.Now;
        return _workService.UpdateAsync(work);
    }

    public Task<IOperationResult> ScheduleWorkAsync(WorkDto work, DateTimeOffset offset)
    {
        if (!work.Instagrams.Any())
            return Task.FromResult((IOperationResult) OperationResult.Fail("Работа не содержит инстаграм(ы)"));
        if (work.IsCompleted)
            return Task.FromResult((IOperationResult) OperationResult.Fail("Работа не может быть запущена"));
        work.JobId =
            BackgroundJob.Schedule(() => _backgroundJobService.ProcessingAsync(work.Id, CancellationToken.None),
                offset);
        BackgroundJob.ContinueJobWith(work.JobId, () => _backgroundJobService.SaveAfterContinuedAsync(work.Id),
            JobContinuationOptions.OnAnyFinishedState);

        work.StartTime = offset.DateTime;
        return _workService.UpdateAsync(work);
    }

    public Task<IOperationResult> CancelWorkAsync(WorkDto work)
    {
        if (string.IsNullOrEmpty(work.JobId))
            return Task.FromResult((IOperationResult) OperationResult.Fail("JobId in null"));
        var result = BackgroundJob.Delete(work.JobId);
        if (!result) OperationResult.Fail("Failed to stop work");
        // work.IsCanceled = true; //TODO :cancel
        return _workService.UpdateAsync(work);
    }
}