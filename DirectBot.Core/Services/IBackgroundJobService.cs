using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IBackgroundJobService
{
    Task ProcessingMailingAsync(WorkDto workDto, CancellationToken token);
    Task<List<WorkDto>> ProcessingDivideWorkAsync(WorkDto workDto, CancellationToken token);
    Task SaveAfterContinuedAsync(WorkDto workDto);
}