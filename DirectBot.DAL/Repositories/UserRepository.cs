using AutoMapper;
using AutoMapper.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using DirectBot.Core.Models;
using DirectBot.Core.Repositories;
using DirectBot.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace DirectBot.DAL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UserRepository(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public Task<List<UserDto>> GetAllAsync()
    {
        return _context.Users.ProjectTo<UserDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public async Task AddOrUpdateAsync(UserDto entity)
    {
        var u = await _context.Users.Persist(_mapper).InsertOrUpdateAsync(entity);
        await _context.SaveChangesAsync();
        entity.Id = u.Id;
    }

    public async Task DeleteAsync(UserDto entity)
    {
        var user = await _context.Users.Persist(_mapper).InsertOrUpdateAsync(entity);
        await _context.Entry(user).Collection(user1 => user1.Instagrams!).Query()
            .Include(instagram => instagram.Works).LoadAsync();
        await _context.Entry(user).Collection(user1 => user1.Subscribes!).LoadAsync();
        _context.RemoveRange(user.Instagrams!.SelectMany(instagram => instagram.Works!));
        _context.RemoveRange(user.Instagrams!);
        _context.RemoveRange(user.Subscribes!);
        _context.Remove(user);
        await _context.SaveChangesAsync();
    }


    public async Task<UserDto?> GetAsync(long id)
    {
        var x = await _context.Users.ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(user => user.Id == id);
        return x;
    }

    public async Task<int> GetCountAsync()
    {
        return await _context.Users.CountAsync();
    }
}