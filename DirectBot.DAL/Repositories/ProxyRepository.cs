using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using AutoMapper.QueryableExtensions;
using DirectBot.Core.DTO;
using DirectBot.Core.Models;
using DirectBot.Core.Repositories;
using DirectBot.DAL.Data;
using DirectBot.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DirectBot.DAL.Repositories;

public class ProxyRepository : IProxyRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ProxyRepository(ApplicationDbContext context)
    {
        _context = context;
        _mapper = GetMapper();
    }

    public async Task AddOrUpdateAsync(ProxyDto entity)
    {
        var u = _mapper.Map<Proxy>(entity);
        await _context.SaveChangesAsync();
        entity.Id = u.Id;
    }

    public async Task DeleteAsync(ProxyDto entity)
    {
        var proxy = _mapper.Map<Proxy>(entity);
        _context.Remove(proxy);
        await _context.SaveChangesAsync();
    }

    public Task<ProxyDto?> GetAsync(int id) =>
        _context.Proxies.ProjectTo<ProxyDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(proxy => proxy.Id == id);

    public Task<ProxyDto?> GetRandomProxyAsync() =>
        _context.Proxies.OrderBy(x => EF.Functions.Random()).ProjectTo<ProxyDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

    public Task<List<ProxyDto>> GetProxiesAsync(ProxySearchQuery query)
    {
        var searchQuery = _context.Proxies.AsQueryable();
        if (!string.IsNullOrEmpty(query.Host))
            searchQuery = searchQuery.Where(proxy => proxy.Host.Contains(query.Host));
        if (query.Port.HasValue)
            searchQuery = searchQuery.Where(proxy => proxy.Port == query.Port);
        if (!string.IsNullOrEmpty(query.Login))
            searchQuery = searchQuery.Where(proxy => proxy.Login.Contains(query.Login));
        if (!string.IsNullOrEmpty(query.Password))
            searchQuery = searchQuery.Where(proxy => proxy.Host.Contains(query.Password));

        return searchQuery.Skip((query.Page - 1) * 30).Take(30).ProjectTo<ProxyDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }


    private IMapper GetMapper()
    {
        return new Mapper(new MapperConfiguration(expr =>
        {
            expr.AddExpressionMapping();
            expr.CreateMap<ProxyDto, Proxy>()
                .ConstructUsing((dto, _) =>
                {
                    if (dto.Id != 0)
                        return _context.Proxies.First(o => o.Id == dto.Id);

                    var proxy = new Proxy();
                    _context.Proxies.Add(proxy);
                    return proxy;
                });
            expr.CreateMap<Proxy, ProxyDto>();
        }));
    }
}