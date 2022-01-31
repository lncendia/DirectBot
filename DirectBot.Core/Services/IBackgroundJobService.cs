namespace DirectBot.Core.Services;

public interface IBackgroundJobService
{
    Task RunAsync(int workId, CancellationToken token);
    Task SaveAfterContinuedAsync(int workId);
}