using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Repositories;

public interface IWorkRepository : IRepository<WorkDTO>
{
    Task<List<WorkDTO>> GetInstagramWorksAsync(InstagramDTO instagram, int page);
    Task<int> GetInstagramWorksCountAsync(InstagramDTO instagram);
    public Task<bool> HasActiveWorksAsync(InstagramDTO instagram);
}