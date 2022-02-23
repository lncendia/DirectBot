using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IInstagramService : IService<InstagramDto, int>
{
     Task<List<InstagramLiteDto>> GetUserInstagramsAsync(long id);
     Task<List<InstagramLiteDto>> GetUserActiveInstagramsAsync(long id);
     Task<int> GetUserInstagramsCountAsync(long id);
     Task<int> GetUserActiveInstagramsCountAsync(long id);
}