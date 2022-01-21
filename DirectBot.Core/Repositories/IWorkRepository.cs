using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Repositories;

public interface IWorkRepository : IRepository<WorkDto, int>
{
    public Task<WorkDto?> GetUserWorksAsync(UserDto userDto, int page);
    Task<int> GetInstagramWorksCountAsync(InstagramDto instagram);
    public Task<bool> HasActiveWorksAsync(InstagramDto instagram);
    public Task<List<WorkDto>> GetUserActiveWorksAsync(UserDto userDto);
}