using DirectBot.Core.DTO;
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

    public Task<List<UserLiteDto>> GetAllAsync() => _userRepository.GetAllAsync();

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

    public Task<UserDto?> GetAsync(long id) => _userRepository.GetAsync(id);

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

    public Task<IOperationResult> AddAsync(UserDto item) => UpdateAsync(item);
    

    public Task<List<UserLiteDto>> GetUsersAsync(UserSearchQuery query) => _userRepository.GetUsersAsync(query);
}