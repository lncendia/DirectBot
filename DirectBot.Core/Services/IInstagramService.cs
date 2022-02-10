using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IInstagramService : IService<InstagramDto, int>
{
     Task<List<InstagramLiteDto>> GetAllAsync();
     Task<List<InstagramLiteDto>> GetUserInstagramsAsync(UserLiteDto user);
     Task<List<InstagramLiteDto>> GetUserActiveInstagramsAsync(UserLiteDto user);
     Task<int> GetUserInstagramsCountAsync(UserLiteDto user);
     Task<int> GetUserActiveInstagramsCountAsync(UserLiteDto user);
}