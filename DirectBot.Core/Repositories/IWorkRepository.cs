using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Repositories;

public interface IWorkRepository : IRepository<WorkDto, int>
{
    Task<List<WorkLiteDto>> GetAllAsync();
    public Task UpdateWithoutStatusAsync(WorkDto entity);
    Task<WorkDto?> GetUserWorksAsync(UserLiteDto userDto, int page);
    Task<bool> HasActiveWorksAsync(InstagramLiteDto instagram);
    Task<bool> IsCancelled(WorkLiteDto workDto);
    Task AddInstagramToWork(WorkLiteDto workDto, InstagramLiteDto instagramDto);
}