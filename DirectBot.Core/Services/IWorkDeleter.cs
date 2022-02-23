namespace DirectBot.Core.Services;

public interface IWorkDeleter
{
    Task StartDeleteAsync();
    void Trigger();
    
}