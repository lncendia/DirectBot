using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Repositories;

public interface IWorkRepository : IRepository<WorkDto, int>
{
    Task<WorkDto?> GetUserWorksAsync(UserDto userDto, int page);
    Task<int> GetInstagramWorksCountAsync(InstagramDto instagram);
    Task<bool> HasActiveWorksAsync(InstagramDto instagram);
    Task<List<WorkDto>> GetUserActiveWorksAsync(UserDto userDto);
}