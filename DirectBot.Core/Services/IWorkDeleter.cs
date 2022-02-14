using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IWorkDeleter
{
    Task StartDeleteAsync();
    void Trigger();
    
}