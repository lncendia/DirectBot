using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface ISubscribeDeleter
{
    Task StartDeleteAsync();
    void Trigger();
    
}