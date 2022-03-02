using DirectBot.Core.Enums;
using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;
using DirectBot.Core.Services;
using Hangfire;

// ReSharper disable MemberCanBePrivate.Global

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

    public Task<IOperationResult> StartWorkNowAsync(WorkDto work)
    {
        if (!work.Instagrams.Any())
            return Task.FromResult((IOperationResult) OperationResult.Fail("Работа не содержит инстаграм(ы)"));
        if (work.IsCompleted)
            return Task.FromResult((IOperationResult) OperationResult.Fail("Работа не может быть запущена"));

        work.JobId = work.Type switch
        {
            WorkType.Simple => BackgroundJob.Enqueue(() => StartSimpleWork(work.Id, CancellationToken.None)),
            WorkType.Divide => BackgroundJob.Enqueue(() => StartDivideWork(work.Id, CancellationToken.None)),
            _ => throw new ArgumentOutOfRangeException()
        };

        BackgroundJob.ContinueJobWith(work.JobId, () => SaveWork(work.Id), JobContinuationOptions.OnAnyFinishedState);
        work.StartTime = DateTime.Now;
        return _workService.UpdateAsync(work);
    }

    public Task<IOperationResult> ScheduleWorkAsync(WorkDto work, DateTimeOffset dateTimeOffset)
    {
        if (!work.Instagrams.Any())
            return Task.FromResult((IOperationResult) OperationResult.Fail("Работа не содержит инстаграм(ы)"));
        if (work.IsCompleted)
            return Task.FromResult((IOperationResult) OperationResult.Fail("Работа не может быть запущена"));

        work.JobId = work.Type switch
        {
            WorkType.Simple => BackgroundJob.Schedule(() => StartSimpleWork(work.Id, CancellationToken.None),
                dateTimeOffset),
            WorkType.Divide => BackgroundJob.Schedule(() => StartDivideWork(work.Id, CancellationToken.None),
                dateTimeOffset),
            _ => throw new ArgumentOutOfRangeException()
        };

        BackgroundJob.ContinueJobWith(work.JobId, () => SaveWork(work.Id), JobContinuationOptions.OnAnyFinishedState);
        work.StartTime = dateTimeOffset.DateTime;
        return _workService.UpdateAsync(work);
    }

    public IOperationResult CancelWorkAsync(WorkDto work)
    {
        if (string.IsNullOrEmpty(work.JobId)) return OperationResult.Fail("JobId in null");
        var result = BackgroundJob.Delete(work.JobId);
        if (!result) OperationResult.Fail("Failed to stop work");
        return OperationResult.Ok();
    }

    [AutomaticRetry(Attempts = 1, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
    public async Task StartDivideWork(int workId, CancellationToken token)
    {
        var work = await _workService.GetAsync(workId);
        if (work == null) return;
        var works = await _backgroundJobService.ProcessingDivideWorkAsync(work, token);
        for (var i = 0; i < works.Count; i++)
        {
            var result = await ScheduleWorkAsync(works[i], DateTimeOffset.Now.Add(work.IntervalPerDivision * i));
            if (!result.Succeeded)
            {
                work.CountErrors++;
                work.ErrorMessage = result.ErrorMessage;
                CancelWorkAsync(works[i]);
                await _workService.DeleteAsync(works[i]);
            }
            else work.CountSuccess++;

            await _workService.UpdateWorkInfoAsync(work);
        }
    }

    [AutomaticRetry(Attempts = 1, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
    public async Task StartSimpleWork(int workId, CancellationToken token)
    {
        var work = await _workService.GetAsync(workId);
        if (work == null) return;
        await _backgroundJobService.ProcessingMailingAsync(work, token);
    }

    [AutomaticRetry(Attempts = 1, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
    public async Task SaveWork(int workId)
    {
        var work = await _workService.GetAsync(workId);
        if (work == null) return;
        await _backgroundJobService.SaveAfterContinuedAsync(work);
    }
}