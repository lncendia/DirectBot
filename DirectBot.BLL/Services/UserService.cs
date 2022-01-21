using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;
using DirectBot.Core.Repositories;
using DirectBot.Core.Services;

namespace DirectBot.BLL.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Task<List<UserDto>> GetAllAsync()
    {
        return _userRepository.GetAllAsync();
    }

    public async Task<IOperationResult> DeleteAsync(UserDto entity)
    {
        try
        {
            await _userRepository.DeleteAsync(entity);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }

    public Task<UserDto?> GetAsync(long id)
    {
        return _userRepository.GetAsync(id);
    }

    public async Task<IOperationResult> UpdateAsync(UserDto entity)
    {
        try
        {
            await _userRepository.AddOrUpdateAsync(entity);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }

    public async Task<IOperationResult> AddAsync(UserDto item)
    {
        try
        {
            await _userRepository.AddOrUpdateAsync(item);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }


    public Task<int> GetCountAsync()
    {
        return _userRepository.GetCountAsync();
    }
}