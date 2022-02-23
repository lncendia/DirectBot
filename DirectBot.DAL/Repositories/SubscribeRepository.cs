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

public class SubscribeRepository : ISubscribeRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public SubscribeRepository(ApplicationDbContext context)
    {
        _context = context;
        _mapper = GetMapper();
    }

    public Task<List<SubscribeDto>> GetAllAsync() =>
        _context.Subscribes.ProjectTo<SubscribeDto>(_mapper.ConfigurationProvider).ToListAsync();

    public async Task AddOrUpdateAsync(SubscribeDto entity)
    {
        var u = _mapper.Map<Subscribe>(entity);
        await _context.SaveChangesAsync();
        entity.Id = u.Id;
    }

    public async Task DeleteAsync(SubscribeDto entity)
    {
        var u = _mapper.Map<Subscribe>(entity);
        _context.Remove(u);
        await _context.SaveChangesAsync();
    }

    public Task<SubscribeDto?> GetAsync(int id) =>
        _context.Subscribes.Include(subscribe => subscribe.User)
            .ProjectTo<SubscribeDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(subscribe => subscribe.Id == id);

    public Task<List<SubscribeDto>> GetUserSubscribesAsync(long id, int page) =>
        _context.Subscribes.Where(payment => payment.User.Id == id && payment.EndSubscribe > DateTime.UtcNow)
            .OrderByDescending(payment => payment.EndSubscribe)
            .Skip((page - 1) * 5).Take(5).ProjectTo<SubscribeDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

    public Task<List<SubscribeDto>> GetSubscribesAsync(SubscribeSearchQuery query)
    {
        var searchQuery = _context.Subscribes.AsQueryable();
        if (query.UserId.HasValue)
            searchQuery = searchQuery.Where(subscribe => subscribe.UserId == query.UserId);
        if (query.EndOfSubscribeLower.HasValue)
            searchQuery = searchQuery.Where(subscribe => subscribe.EndSubscribe >= query.EndOfSubscribeLower);
        if (query.EndOfSubscribeUpper.HasValue)
            searchQuery = searchQuery.Where(subscribe => subscribe.EndSubscribe <= query.EndOfSubscribeUpper);
        return searchQuery.Skip((query.Page - 1) * 30).Take(30).ProjectTo<SubscribeDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public Task<int> GetUserSubscribesCountAsync(long id) =>
        _context.Subscribes.Where(payment => payment.User.Id == id && payment.EndSubscribe > DateTime.UtcNow)
            .CountAsync();

    public Task<List<SubscribeDto>> GetExpiredSubscribes() =>
        _context.Subscribes.Include(subscribe => subscribe.User)
            .Where(subscribe => subscribe.EndSubscribe < DateTime.UtcNow)
            .ProjectTo<SubscribeDto>(_mapper.ConfigurationProvider).ToListAsync();

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
            expr.CreateMap<SubscribeDto, Subscribe>()
                .ConstructUsing((dto, _) =>
                {
                    if (dto.Id != 0)
                        return _context.Subscribes.Include(subscribe => subscribe.User).First(o => o.Id == dto.Id);

                    var detail = new Subscribe();
                    _context.Subscribes.Add(detail);
                    return detail;
                });

            expr.CreateMap<User, UserLiteDto>();
            expr.CreateMap<Subscribe, SubscribeDto>();
        }));
    }
}