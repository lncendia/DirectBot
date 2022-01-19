using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;
using DirectBot.Core.Repositories;
using DirectBot.Core.Services;

namespace DirectBot.BLL.Services;

public class InstagramService : IInstagramService
{
    private readonly IInstagramRepository _instagramRepository;

    public InstagramService(IInstagramRepository instagramRepository)
    {
        _instagramRepository = instagramRepository;
    }

    public Task<List<InstagramDTO>> GetAllAsync()
    {
        return _instagramRepository.GetAllAsync();
    }

    public async Task<IOperationResult> DeleteAsync(InstagramDTO entity)
    {
        try
        {
            await _instagramRepository.DeleteAsync(entity);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }

    public Task<InstagramDTO?> GetAsync(long id)
    {
        return _instagramRepository.GetAsync(id);
    }


    public async Task<IOperationResult> UpdateAsync(InstagramDTO instagram)
    {
        try
        {
            await _instagramRepository.UpdateAsync(instagram);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }

    public async Task<IOperationResult> AddAsync(InstagramDTO item)
    {
        try
        {
            var instagrams = await _instagramRepository.GetUserInstagramsAsync(item.User);
            if (instagrams.Any(instagram => instagram.Username == item.Username))
                return OperationResult.Fail("Инстаграм с таким именем уже существуют");
            await _instagramRepository.AddAsync(item);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }

    public Task<List<InstagramDTO>> GetUserInstagramsAsync(UserDTO user)
    {
        return _instagramRepository.GetUserInstagramsAsync(user);
    }

    public Task<int> GetUserInstagramsCountAsync(UserDTO user)
    {
        return _instagramRepository.GetUserInstagramsCountAsync(user);
    }
}