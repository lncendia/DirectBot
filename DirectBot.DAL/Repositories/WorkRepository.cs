using DirectBot.Core.Models;
using DirectBot.Core.Repositories;
using DirectBot.DAL.Data;
using Microsoft.EntityFrameworkCore;
using Instagram = DirectBot.DAL.Models.Instagram;
using Work = DirectBot.DAL.Models.Work;

namespace DirectBot.DAL.Repositories;

public class WorkRepository : IWorkRepository
{
    private readonly ApplicationDbContext _context;

    public WorkRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<Work>> GetAllAsync()
    {
        return _context.Works.ToListAsync();
    }

    public async Task AddAsync(Work entity)
    {
        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Work entity)
    {
        _context.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Work entity)
    {
        await _context.SaveChangesAsync();
    }

    public Task<Work?> GetAsync(long id)
    {
        return _context.Works.Include(work => work.Instagram.User).FirstOrDefaultAsync(work => work.Id == id);
    }

    public async Task<List<Work>> GetInstagramWorksAsync(Instagram user, int page)
    {
        return await _context.Works.Where(work => work.Instagram == user)
            .Skip((page - 1) * 30).Take(30).ToListAsync();
    }

    public Task<int> GetInstagramWorksCountAsync(Instagram instagram)
    {
        return _context.Works.Where(work => work.Instagram == instagram).CountAsync();
    }

    public Task<bool> HasActiveWorksAsync(Instagram instagram)
    {
        return _context.Works.AnyAsync(work => work.Instagram == instagram && !work.IsCompleted);
    }
}