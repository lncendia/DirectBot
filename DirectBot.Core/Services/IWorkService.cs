using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IWorkService : IService<WorkDto, int>
{
    public Task<IOperationResult> UpdateWorkInfoAsync(WorkDto entity);
    Task<WorkDto?> GetUserWorksAsync(long id, int page);
    Task<bool> HasActiveWorksAsync(int id);
    Task<int> GetInstagramsCountAsync(int id);
    Task<List<WorkDto>> GetExpiredSubscribes();
}