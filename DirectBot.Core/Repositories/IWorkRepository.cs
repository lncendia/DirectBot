using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Repositories;

public interface IWorkRepository : IRepository<WorkDto, int>
{
    public Task UpdateWithoutStatusAsync(WorkDto entity);
    Task<WorkDto?> GetUserWorksAsync(UserDto userDto, int page);
    Task<int> GetInstagramWorksCountAsync(InstagramDto instagram);
    Task<bool> HasActiveWorksAsync(InstagramDto instagram);
    Task<List<WorkDto>> GetUserActiveWorksAsync(UserDto userDto);
    Task<bool> IsCancelled(WorkDto workDto);
}