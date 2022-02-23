namespace DirectBot.Core.Services;

public interface ISubscribeDeleter
{
    Task StartDeleteAsync();
    void Trigger();
    
}