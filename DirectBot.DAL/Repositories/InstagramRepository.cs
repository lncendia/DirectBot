using AutoMapper;
using AutoMapper.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using DirectBot.Core.Models;
using DirectBot.Core.Repositories;
using DirectBot.DAL.Data;
using DirectBot.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DirectBot.DAL.Repositories;

public class InstagramRepository : IInstagramRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public InstagramRepository(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public Task<List<InstagramDto>> GetAllAsync() =>
        _context.Instagrams.ProjectTo<InstagramDto>(_mapper.ConfigurationProvider).ToListAsync();

    public async Task AddOrUpdateAsync(InstagramDto entity)
    {
        var u = await _context.Instagrams.Persist(_mapper).InsertOrUpdateAsync(entity);
        await _context.SaveChangesAsync();
        entity.Id = u.Id;
    }

    public async Task DeleteAsync(InstagramDto entity)
    {
        await _context.Instagrams.Persist(_mapper).RemoveAsync(entity);
        await _context.SaveChangesAsync();
    }

    public Task<InstagramDto?> GetAsync(int id)
    {
        return _context.Instagrams.Include(instagram => instagram.User)
            .ProjectTo<InstagramDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(instagram => instagram.Id == id);
    }

    public Task<List<InstagramDto>> GetUserInstagramsAsync(UserDto user, bool onlyActive = false)
    {
        var query = _context.Instagrams.Include(instagram => instagram.Proxy)
            .Where(instagram => instagram.User.Id == user.Id);
        if (onlyActive) query = query.Where(instagram => instagram.IsActive);
        return query.ProjectTo<InstagramDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public Task<int> GetUserInstagramsCountAsync(UserDto user, bool onlyActive = false)
    {
        var query = _context.Instagrams.AsQueryable();
        if (onlyActive) query = query.Where(instagram => instagram.IsActive);
        return query.CountAsync(instagram => instagram.User.Id == user.Id);
    }

    public Task<InstagramDto?> GetUserInstagramsAsync(UserDto user, int page) =>
        _context.Instagrams.Where(instagram => instagram.UserId == user.Id)
            .Skip(page - 1).ProjectTo<InstagramDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
}