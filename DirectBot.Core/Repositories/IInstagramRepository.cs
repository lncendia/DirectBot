using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Repositories;

public interface IInstagramRepository : IRepository<InstagramDto, int>
{
    Task<List<InstagramLiteDto>> GetUserInstagramsAsync(long id, bool onlyActive = false);
    Task<int> GetUserInstagramsCountAsync(long id, bool onlyActive = false);
}