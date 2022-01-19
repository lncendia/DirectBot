using DirectBot.Core.Models;
using DirectBot.Core.Repositories;
using DirectBot.DAL.Data;
using Microsoft.EntityFrameworkCore;
using Proxy = DirectBot.DAL.Models.Proxy;

namespace DirectBot.DAL.Repositories;

public class ProxyRepository : IProxyRepository
{
    private readonly ApplicationDbContext _context;

    public ProxyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<Proxy>> GetAllAsync()
    {
        return _context.Proxies.ToListAsync();
    }

    public async Task AddAsync(Proxy entity)
    {
        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Proxy entity)
    {
        _context.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Proxy entity)
    {
        await _context.SaveChangesAsync();
    }

    public Task<Proxy?> GetAsync(long id)
    {
        return _context.Proxies.FirstOrDefaultAsync(proxy => proxy.Id == id);
    }

    public Task<Proxy?> GetRandomProxyAsync()
    {
        return _context.Proxies.OrderBy(x=>EF.Functions.Random()).FirstOrDefaultAsync();
    }
}