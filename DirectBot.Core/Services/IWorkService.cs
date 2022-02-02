using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IWorkService : IService<WorkDto, int>
{
     public Task<IOperationResult> UpdateWithoutStatusAsync(WorkDto entity);
     Task<WorkDto?> GetUserWorksAsync(UserDto userDto, int page);
     Task<int> GetInstagramWorksCountAsync(InstagramDto instagram);
     Task<List<WorkDto>> GetUserActiveWorksAsync(UserDto userDto);
     Task<bool> HasActiveWorksAsync(InstagramDto instagram);
     Task<bool> IsCancelled(WorkDto workDto);
}