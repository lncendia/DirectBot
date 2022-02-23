using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;
using DirectBot.Core.Repositories;
using DirectBot.Core.Services;

namespace DirectBot.BLL.Services;

public class InstagramService : IInstagramService
{
    private readonly IInstagramRepository _instagramRepository;

    public InstagramService(IInstagramRepository instagramRepository) => _instagramRepository = instagramRepository;

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

    public Task<InstagramDto?> GetAsync(int id) => _instagramRepository.GetAsync(id);


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
        var instagrams = await _instagramRepository.GetUserInstagramsAsync(item.Id);
        if (instagrams.Any(instagram => instagram.Username == item.Username))
            return OperationResult.Fail("Инстаграм с таким именем уже существуют");
        return await UpdateAsync(item);
    }

    public Task<List<InstagramLiteDto>> GetUserInstagramsAsync(long id) =>
        _instagramRepository.GetUserInstagramsAsync(id);

    public Task<List<InstagramLiteDto>> GetUserActiveInstagramsAsync(long id) =>
        _instagramRepository.GetUserInstagramsAsync(id, true);


    public Task<int> GetUserInstagramsCountAsync(long id) =>
        _instagramRepository.GetUserInstagramsCountAsync(id);

    public Task<int> GetUserActiveInstagramsCountAsync(long id) =>
        _instagramRepository.GetUserInstagramsCountAsync(id, true);
}