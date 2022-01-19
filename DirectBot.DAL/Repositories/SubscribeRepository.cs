using DirectBot.Core.Models;
using DirectBot.Core.Repositories;
using DirectBot.DAL.Data;
using Microsoft.EntityFrameworkCore;
using Subscribe = DirectBot.DAL.Models.Subscribe;
using User = DirectBot.DAL.Models.User;

namespace DirectBot.DAL.Repositories;

public class SubscribeRepository : ISubscribeRepository
{
    private readonly ApplicationDbContext _context;

    public SubscribeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<Subscribe>> GetAllAsync()
    {
        return _context.Subscribes.ToListAsync();
    }

    public async Task AddAsync(Subscribe entity)
    {
        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Subscribe entity)
    {
        _context.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Subscribe entity)
    {
        await _context.SaveChangesAsync();
    }

    public Task<Subscribe?> GetAsync(long id)
    {
        return _context.Subscribes.FirstOrDefaultAsync(subscribe => subscribe.Id == id);
    }

    public async Task<List<Subscribe>> GetUserSubscribesAsync(User user, int page)
    {
        return await _context.Subscribes.Where(payment => payment.User == user)
            .Skip((page - 1) * 30).Take(30)
            .OrderByDescending(payment => payment.EndSubscribe).ToListAsync();
    }

    public Task<int> GetUserSubscribesCountAsync(User user)
    {
        return _context.Subscribes.Where(payment => payment.User == user).CountAsync();
    }
}