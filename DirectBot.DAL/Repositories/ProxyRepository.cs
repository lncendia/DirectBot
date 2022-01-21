using AutoMapper;
using AutoMapper.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using DirectBot.Core.Models;
using DirectBot.Core.Repositories;
using DirectBot.DAL.Data;
using Microsoft.EntityFrameworkCore;
using Proxy = DirectBot.DAL.Models.Proxy;

namespace DirectBot.DAL.Repositories;

public class ProxyRepository : IProxyRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ProxyRepository(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public Task<List<ProxyDto>> GetAllAsync()
    {
        return _context.Proxies.ProjectTo<ProxyDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public async Task AddOrUpdateAsync(ProxyDto entity)
    {
       var u =  await _context.Proxies.Persist(_mapper).InsertOrUpdateAsync(entity);
        await _context.SaveChangesAsync();
        entity.Id = u.Id;
    }

    public async Task DeleteAsync(ProxyDto entity)
    {
        await _context.Proxies.Persist(_mapper).RemoveAsync(entity);
        await _context.SaveChangesAsync();
    }

    public Task<ProxyDto?> GetAsync(int id)
    {
        return _context.Proxies.ProjectTo<ProxyDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(proxy => proxy.Id == id);
    }

    public Task<ProxyDto?> GetRandomProxyAsync()
    {
        return _context.Proxies.OrderBy(x => EF.Functions.Random()).ProjectTo<ProxyDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }
}