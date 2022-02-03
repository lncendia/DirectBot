using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Repositories;

public interface IWorkRepository : IRepository<WorkDto, int>
{
    public Task UpdateWithoutStatusAsync(WorkDto entity);
    Task<WorkDto?> GetUserWorksAsync(UserDto userDto, int page);
    Task<bool> HasActiveWorksAsync(InstagramDto instagram);
    Task<WorkDto?> GetUserSelectedWorkAsync(UserDto userDto);
    Task<bool> IsCancelled(WorkDto workDto);
    Task AddInstagramToWork(WorkDto workDto, InstagramDto instagramDto);
}