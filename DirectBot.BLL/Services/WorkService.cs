using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;
using DirectBot.Core.Repositories;
using DirectBot.Core.Services;

namespace DirectBot.BLL.Services;

public class WorkService : IWorkService
{
    private readonly IWorkRepository _workRepository;

    public WorkService(IWorkRepository workRepository) => _workRepository = workRepository;

    public Task<WorkDto?> GetUserWorksAsync(long id, int page) => _workRepository.GetUserWorksAsync(id, page);

    public Task<bool> HasActiveWorksAsync(int id) => _workRepository.HasActiveWorksAsync(id);
    public Task<int> GetInstagramsCountAsync(int id) => _workRepository.GetInstagramsCountAsync(id);

    public Task<List<WorkDto>> GetExpiredSubscribes() => _workRepository.GetExpiredWorks();

    public Task<WorkDto?> GetAsync(int id) => _workRepository.GetAsync(id);

    public async Task<IOperationResult> UpdateAsync(WorkDto entity)
    {
        try
        {
            await _workRepository.AddOrUpdateAsync(entity);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }

    public async Task<IOperationResult> UpdateWithoutStatusAsync(WorkDto entity)
    {
        try
        {
            await _workRepository.UpdateProcessingInfoAsync(entity);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }

    public Task<IOperationResult> AddAsync(WorkDto item) => UpdateAsync(item);

    public async Task<IOperationResult> DeleteAsync(WorkDto work)
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