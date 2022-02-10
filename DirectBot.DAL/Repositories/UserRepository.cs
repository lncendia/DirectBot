using AutoMapper;
using AutoMapper.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using DirectBot.Core.DTO;
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

    public Task<List<UserLiteDto>> GetAllAsync() =>
        _context.Users.ProjectTo<UserLiteDto>(_mapper.ConfigurationProvider).ToListAsync();

    public async Task AddOrUpdateAsync(UserDto entity)
    {
        var u = await _context.Users.Persist(_mapper).InsertOrUpdateAsync(entity);
        await _context.SaveChangesAsync();
        entity.Id = u.Id;
    }

    public async Task DeleteAsync(UserDto entity)
    {
        await _context.Users.Persist(_mapper).RemoveAsync(entity);
        // await _context.Entry(user).Collection(user1 => user1.Instagrams!).Query()
        //     .Include(instagram => instagram.Works).LoadAsync();
        // await _context.Entry(user).Collection(user1 => user1.Subscribes!).LoadAsync();
        // await _context.Entry(user).Collection(user1 => user1.Payments!).LoadAsync();
        await _context.SaveChangesAsync();
    }


    public async Task<UserDto?> GetAsync(long id)
    {
        var x = await _context.Users //.Include(user => user.CurrentInstagram).Include(user => user.CurrentWork)
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(user => user.Id == id);
        return x;
    }

    public Task<int> GetCountAsync() => _context.Users.CountAsync();

    public Task<List<UserDto>> GetUsersAsync(UserSearchQuery query)
    {
        var searchQuery = _context.Users.AsQueryable()
            .Where(user => user.IsAdmin == query.IsAdmin && user.IsBanned == query.IsBanned);
        if (query.UserId.HasValue)
            searchQuery = searchQuery.Where(user => user.Id == query.UserId);
        if (query.State.HasValue)
            searchQuery = searchQuery.Where(user => user.State == query.State);
        return searchQuery.Skip((query.Page - 1) * 30).Take(30).ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }
}