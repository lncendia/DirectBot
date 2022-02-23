using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Repositories;

public interface IWorkRepository : IRepository<WorkDto, int>
{
    Task UpdateProcessingInfoAsync(WorkDto entity);
    Task<WorkDto?> GetUserWorksAsync(long id, int page);
    Task<bool> HasActiveWorksAsync(int instagramId);
    Task<int> GetInstagramsCountAsync(int id);
    Task<List<WorkDto>> GetExpiredWorks();
}