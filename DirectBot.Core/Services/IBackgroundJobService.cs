namespace DirectBot.Core.Services;

public interface IBackgroundJobService
{
    Task ProcessingAsync(int workId, CancellationToken token);
    Task SaveAfterContinuedAsync(int workId);
}