using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface ISubscribeNotifier
{
    Task NotifyStartAsync();
    void Trigger();
    
}