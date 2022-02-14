using DirectBot.Core.Services;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace DirectBot.BLL.Services;

public class WorkDeleter : IWorkDeleter
{
    private readonly IWorkService _workService;
    private readonly ILogger<WorkDeleter> _logger;

    public WorkDeleter(IWorkService workService, ILogger<WorkDeleter> logger)
    {
        _workService = workService;
        _logger = logger;
    }

    public async Task StartDeleteAsync()
    {
        var works = await _workService.GetExpiredSubscribes();
        foreach (var work in works)
        {
            var result = await _workService.DeleteAsync(work);
            if (!result.Succeeded)
                _logger.LogError("Error deleting work {ErrorMessage}", result.ErrorMessage);
        }
    }

    public void Trigger()
    {
        RecurringJob.Trigger("worksChecker");
    }
}