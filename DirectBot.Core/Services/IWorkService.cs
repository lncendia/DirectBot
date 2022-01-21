using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IWorkService : IService<WorkDto, int>
{
    public Task<WorkDto?> GetUserWorksAsync(UserDto userDto, int page);
    public Task<int> GetInstagramWorksCountAsync(InstagramDto instagram);
    public Task<List<WorkDto>> GetUserActiveWorksAsync(UserDto userDto);
    public Task<bool> HasActiveWorksAsync(InstagramDto instagram);
}