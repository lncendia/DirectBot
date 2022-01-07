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

    public List<User> GetAll()
    {
        return _userRepository.GetAll();
    }

    public IOperationResult Delete(User entity)
    {
        try
        {
            _userRepository.Delete(entity);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }

    public User? Get(long id)
    {
        return _userRepository.Get(id);
    }

    public void Update(User entity)
    {
        _userRepository.Update(entity);
    }

    public IOperationResult Add(User item)
    {
        try
        {
            _userRepository.Add(item);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }
    

    public IResult<int> GetCount()
    {
        try
        {
            return Result<int>.Ok(_userRepository.GetCount());
        }
        catch (Exception ex)
        {
            return Result<int>.Fail(ex.Message);
        }
    }
}