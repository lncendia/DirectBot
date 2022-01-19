using DirectBot.Core.Models;
using DirectBot.Core.Repositories;
using DirectBot.DAL.Data;
using Microsoft.EntityFrameworkCore;
using User = DirectBot.DAL.Models.User;

namespace DirectBot.DAL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<User>> GetAllAsync()
    {
        return _context.Users.ToListAsync();
    }

    public async Task AddAsync(User entity)
    {
        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(User entity)
    {
        await _context.Entry(entity).Collection(user1 => user1.Instagrams!).Query()
            .Include(instagram => instagram.Works).LoadAsync();
        await _context.Entry(entity).Collection(user1 => user1.Subscribes!).LoadAsync();
        _context.RemoveRange(entity.Instagrams!.SelectMany(instagram => instagram.Works!));
        _context.RemoveRange(entity.Instagrams!);
        _context.RemoveRange(entity.Subscribes!);
        _context.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User entity)
    {
        await _context.SaveChangesAsync();
    }

    public Task<User?> GetAsync(long id)
    {
        return _context.Users.Include(user => user.CurrentInstagram).Include(user => user.CurrentWorks)
            .FirstOrDefaultAsync(user => user.Id == id);
    }

    public async Task<int> GetCountAsync()
    {
        return await _context.Users.CountAsync();
    }
}