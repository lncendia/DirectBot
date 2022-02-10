using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Repositories;

public interface IInstagramRepository : IRepository<InstagramDto, int>
{
    Task<List<InstagramLiteDto>> GetAllAsync();
    Task<List<InstagramLiteDto>> GetUserInstagramsAsync(UserLiteDto user, bool onlyActive = false);
    Task<int> GetUserInstagramsCountAsync(UserLiteDto user, bool onlyActive = false);
}