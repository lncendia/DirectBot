using AutoMapper;
using AutoMapper.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using DirectBot.Core.Models;
using DirectBot.Core.Repositories;
using DirectBot.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace DirectBot.DAL.Repositories;

public class SubscribeRepository : ISubscribeRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public SubscribeRepository(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public Task<List<SubscribeDto>> GetAllAsync()
    {
        return _context.Subscribes.ProjectTo<SubscribeDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public async Task AddOrUpdateAsync(SubscribeDto entity)
    {
        var u = await _context.Subscribes.Persist(_mapper).InsertOrUpdateAsync(entity);
        await _context.SaveChangesAsync();
        entity.Id = u.Id;
    }

    public async Task DeleteAsync(SubscribeDto entity)
    {
        await _context.Subscribes.Persist(_mapper).RemoveAsync(entity);
        await _context.SaveChangesAsync();
    }

    public Task<SubscribeDto?> GetAsync(int id)
    {
        return _context.Subscribes.Include(subscribe => subscribe.User)
            .ProjectTo<SubscribeDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(subscribe => subscribe.Id == id);
    }

    public Task<List<SubscribeDto>> GetUserSubscribesAsync(UserDto user, int page)
    {
        return _context.Subscribes.Where(payment => payment.User.Id == user.Id)
            .Skip((page - 1) * 5).Take(5)
            .OrderByDescending(payment => payment.EndSubscribe).ProjectTo<SubscribeDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public Task<int> GetUserSubscribesCountAsync(UserDto user)
    {
        return _context.Subscribes.Where(payment => payment.User.Id == user.Id).CountAsync();
    }
}