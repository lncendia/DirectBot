using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
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

    public InstagramRepository(ApplicationDbContext context)
    {
        _context = context;
        _mapper = GetMapper();
    }

    public async Task AddOrUpdateAsync(InstagramDto entity)
    {
        var u = _mapper.Map<Instagram>(entity);
        await _context.SaveChangesAsync();
        entity.Id = u.Id;
    }

    public async Task DeleteAsync(InstagramDto entity)
    {
        var u = _mapper.Map<Instagram>(entity);
        _context.Remove(u);
        await _context.SaveChangesAsync();
    }

    public Task<InstagramDto?> GetAsync(int id)
    {
        return _context.Instagrams.Include(instagram => instagram.User)
            .ProjectTo<InstagramDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(instagram => instagram.Id == id);
    }

    public Task<List<InstagramLiteDto>> GetUserInstagramsAsync(long id, bool onlyActive = false)
    {
        var query = _context.Instagrams.Include(instagram => instagram.Proxy)
            .Where(instagram => instagram.User.Id == id);
        if (onlyActive) query = query.Where(instagram => instagram.IsActive);
        return query.ProjectTo<InstagramLiteDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public Task<int> GetUserInstagramsCountAsync(long id, bool onlyActive = false)
    {
        var query = _context.Instagrams.AsQueryable();
        if (onlyActive) query = query.Where(instagram => instagram.IsActive);
        return query.CountAsync(instagram => instagram.User.Id == id);
    }

    private IMapper GetMapper()
    {
        return new Mapper(new MapperConfiguration(expr =>
        {
            expr.AddExpressionMapping();
            expr.CreateMap<UserLiteDto, User>()
                .ConstructUsing((dto, _) =>
                {
                    if (dto.Id != 0)
                        return _context.Users.First(o => o.Id == dto.Id);

                    var user = new User();
                    _context.Users.Add(user);
                    return user;
                });
            expr.CreateMap<ProxyDto, Proxy>()
                .ConstructUsing((dto, _) =>
                {
                    if (dto.Id != 0)
                        return _context.Proxies.First(o => o.Id == dto.Id);

                    var proxy = new Proxy();
                    _context.Add(proxy);
                    return proxy;
                });
            expr.CreateMap<InstagramDto, Instagram>()
                .ConstructUsing((dto, _) =>
                {
                    if (dto.Id != 0)
                        return _context.Instagrams.Include(instagram => instagram.User)
                            .Include(instagram => instagram.Proxy).First(o => o.Id == dto.Id);

                    var detail = new Instagram();
                    _context.Instagrams.Add(detail);
                    return detail;
                });


            expr.CreateMap<Instagram, InstagramDto>();
            expr.CreateMap<Proxy, ProxyDto>();
            expr.CreateMap<Instagram, InstagramLiteDto>();
            expr.CreateMap<User, UserLiteDto>();
        }));
    }
}