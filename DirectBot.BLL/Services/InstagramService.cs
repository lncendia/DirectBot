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

    public Task<List<InstagramDto>> GetAllAsync()
    {
        return _instagramRepository.GetAllAsync();
    }

    public async Task<IOperationResult> DeleteAsync(InstagramDto entity)
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

    public Task<InstagramDto?> GetAsync(int id)
    {
        return _instagramRepository.GetAsync(id);
    }


    public async Task<IOperationResult> UpdateAsync(InstagramDto instagram)
    {
        try
        {
            await _instagramRepository.AddOrUpdateAsync(instagram);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }

    public async Task<IOperationResult> AddAsync(InstagramDto item)
    {
        try
        {
            var instagrams = await _instagramRepository.GetUserInstagramsAsync(item.User!);
            if (instagrams.Any(instagram => instagram.Username == item.Username))
                return OperationResult.Fail("Инстаграм с таким именем уже существуют");
            await _instagramRepository.AddOrUpdateAsync(item);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }

    public Task<List<InstagramDto>> GetUserInstagramsAsync(UserDto user)
    {
        return _instagramRepository.GetUserInstagramsAsync(user);
    }

    public Task<List<InstagramDto>> GetUserActiveInstagramsAsync(UserDto user)
    {
        return _instagramRepository.GetUserInstagramsAsync(user, true);
    }

    public Task<InstagramDto?> GetUserInstagramsAsync(UserDto user, int page)
    {
        return _instagramRepository.GetUserInstagramsAsync(user, page);
    }

    public Task<int> GetUserInstagramsCountAsync(UserDto user)
    {
        return _instagramRepository.GetUserInstagramsCountAsync(user);
    }

    public Task<int> GetUserActiveInstagramsCountAsync(UserDto user)
    {
        return _instagramRepository.GetUserInstagramsCountAsync(user, true);
    }

    public Task<InstagramDto?> GetUserSelectedInstagramAsync(UserDto userDto)
    {
        return _instagramRepository.GetUserSelectedInstagramAsync(userDto);
    }
}