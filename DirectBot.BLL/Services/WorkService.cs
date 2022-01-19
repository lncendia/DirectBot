using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;
using DirectBot.Core.Repositories;
using DirectBot.Core.Services;

namespace DirectBot.BLL.Services;

public class WorkService : IWorkService
{
    private readonly IWorkRepository _workRepository;

    public WorkService(IWorkRepository workRepository)
    {
        _workRepository = workRepository;
    }

    public Task<List<WorkDTO>> GetInstagramWorksAsync(InstagramDTO instagram, int page)
    {
        return _workRepository.GetInstagramWorksAsync(instagram, page);
    }

    public Task<int> GetInstagramWorksCountAsync(InstagramDTO instagram)
    {
        return _workRepository.GetInstagramWorksCountAsync(instagram);
    }

    public Task<bool> HasActiveWorksAsync(InstagramDTO instagram)
    {
        return _workRepository.HasActiveWorksAsync(instagram);
    }

    public Task<WorkDTO?> GetAsync(long id)
    {
        return _workRepository.GetAsync(id);
    }

    public async Task<IOperationResult> UpdateAsync(WorkDTO entity)
    {
        try
        {
            await _workRepository.UpdateAsync(entity);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }

    public async Task<IOperationResult> AddAsync(WorkDTO item)
    {
        try
        {
            await _workRepository.AddAsync(item);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }

    public Task<List<WorkDTO>> GetAllAsync()
    {
        return _workRepository.GetAllAsync();
    }

    public async Task<IOperationResult> DeleteAsync(WorkDTO work)
    {
        try
        {
            if (!string.IsNullOrEmpty(work.JobId) && work.IsCompleted == false)
            {
                return OperationResult.Fail($"Работа {work.Id} ещё не завершена.");
            }

            await _workRepository.DeleteAsync(work);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }
}