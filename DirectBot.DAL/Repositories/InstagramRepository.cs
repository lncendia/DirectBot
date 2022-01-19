using AutoMapper;
using DirectBot.Core.Models;
using DirectBot.Core.Repositories;
using DirectBot.DAL.Data;
using DirectBot.DAL.Models;
using Microsoft.EntityFrameworkCore;
using User = DirectBot.DAL.Models.User;

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

    public async Task<List<InstagramDTO>> GetAllAsync()
    {
        return _mapper.Map<List<InstagramDTO>>(await _context.Instagrams.ToListAsync());
    }

    public async Task AddAsync(InstagramDTO entity)
    {
        await _context.AddAsync(_mapper.Map<Instagram>(entity));
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(InstagramDTO entity)
    {
        var inst = _mapper.Map<Instagram>(entity);
        _context.Update(inst);
        _context.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(InstagramDTO entity)
    {
        await _context.SaveChangesAsync();
    }

    public Task<InstagramDTO?> GetAsync(long id)
    {
        return _context.Instagrams.Include(instagram => instagram.User)
            .FirstOrDefaultAsync(instagram => instagram.Id == id);
    }

    public Task<List<InstagramDTO>> GetUserInstagramsAsync(User user)
    {
        return _context.Instagrams.Include(instagram => instagram.Proxy).Where(instagram => instagram.User == user)
            .ToListAsync();
    }

    public Task<int> GetUserInstagramsCountAsync(User user)
    {
        return _context.Instagrams.CountAsync(instagram => instagram.User == user);
    }
}