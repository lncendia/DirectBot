using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;
using DirectBot.Core.Repositories;
using DirectBot.Core.Services;

namespace DirectBot.BLL.Services;

public class WorkService : IWorkService
{
    private readonly IWorkRepository _workRepository;

    public WorkService(IWorkRepository workRepository) => _workRepository = workRepository;

    public Task<WorkDto?> GetUserWorksAsync(UserDto userDto, int page) =>
        _workRepository.GetUserWorksAsync(userDto, page);

    public Task<bool> HasActiveWorksAsync(InstagramDto instagram) => _workRepository.HasActiveWorksAsync(instagram);
    public Task<bool> IsCancelled(WorkDto workDto) => _workRepository.IsCancelled(workDto);

    public async Task<IOperationResult> AddInstagramToWork(WorkDto workDto, InstagramDto instagramDto)
    {
        try
        {
            await _workRepository.AddInstagramToWork(workDto, instagramDto);
            workDto.Instagrams.Add(new InstagramLiteDto
            {
                Id = instagramDto.Id, Username = instagramDto.Username, IsActive = instagramDto.IsActive
            });
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }

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
            await _workRepository.AddOrUpdateAsync(entity);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }

    public Task<IOperationResult> AddAsync(WorkDto item) => UpdateAsync(item);

    public Task<List<WorkDto>> GetAllAsync() => _workRepository.GetAllAsync();

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