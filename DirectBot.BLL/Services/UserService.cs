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

    public Task<List<UserDTO>> GetAllAsync()
    {
        return _userRepository.GetAllAsync();
    }

    public async Task<IOperationResult> DeleteAsync(UserDTO entity)
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

    public Task<UserDTO?> GetAsync(long id)
    {
        return _userRepository.GetAsync(id);
    }

    public async Task<IOperationResult> UpdateAsync(UserDTO entity)
    {
        try
        {
            await _userRepository.UpdateAsync(entity);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }

    public async Task<IOperationResult> AddAsync(UserDTO item)
    {
        try
        {
            await _userRepository.AddAsync(item);
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