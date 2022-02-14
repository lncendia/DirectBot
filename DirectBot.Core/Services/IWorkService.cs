using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IWorkService : IService<WorkDto, int>
{
    Task<List<WorkLiteDto>> GetAllAsync();
    public Task<IOperationResult> UpdateWithoutStatusAsync(WorkDto entity);
    Task<WorkDto?> GetUserWorksAsync(UserLiteDto userDto, int page);
    Task<bool> HasActiveWorksAsync(InstagramLiteDto instagram);
    Task<bool> IsCancelled(WorkLiteDto workDto);
    Task<IOperationResult> AddInstagramToWork(WorkLiteDto workDto, InstagramLiteDto instagramDto);
    Task<List<WorkDto>> GetExpiredSubscribes();
}