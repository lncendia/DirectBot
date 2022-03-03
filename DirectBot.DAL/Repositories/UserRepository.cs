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

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
        _mapper = GetMapper();
    }

    public Task<List<UserLiteDto>> GetAllAsync() =>
        _context.Users.ProjectTo<UserLiteDto>(_mapper.ConfigurationProvider).ToListAsync();

    public async Task AddOrUpdateAsync(UserDto entity)
    {
        var u = _mapper.Map<User>(entity);
        await _context.SaveChangesAsync();
        entity.Id = u.Id;
    }

    public async Task DeleteAsync(UserDto entity)
    {
        var user = _mapper.Map<User>(entity);
        await _context.Entry(user).Collection(user1 => user1.Instagrams!).Query().Include(instagram => instagram.Works)
            .LoadAsync();
        var works = _context.Works.Where(work => work.Instagrams.Any(instagram => instagram.User == user));
        _context.RemoveRange(works);
        _context.Remove(user);
        await _context.SaveChangesAsync();
    }


    public async Task<UserDto?> GetAsync(long id)
    {
        var x = await _context.Users.ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(user => user.Id == id);
        return x;
    }

    public Task<List<UserLiteDto>> GetUsersAsync(UserSearchQuery query)
    {
        var searchQuery = _context.Users.AsQueryable()
            .Where(user => user.IsAdmin == query.IsAdmin && user.IsBanned == query.IsBanned);
        if (query.UserId.HasValue)
            searchQuery = searchQuery.Where(user => user.Id == query.UserId);
        if (query.State.HasValue)
            searchQuery = searchQuery.Where(user => user.State == query.State);
        return searchQuery.Skip((query.Page - 1) * 30).Take(30).ProjectTo<UserLiteDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public Task<int> GetUsersCountAsync(UserSearchQuery query)
    {
        var searchQuery = _context.Users.AsQueryable()
            .Where(user => user.IsAdmin == query.IsAdmin && user.IsBanned == query.IsBanned);
        if (query.UserId.HasValue)
            searchQuery = searchQuery.Where(user => user.Id == query.UserId);
        if (query.State.HasValue)
            searchQuery = searchQuery.Where(user => user.State == query.State);
        return searchQuery.CountAsync();
    }

    private IMapper GetMapper()
    {
        return new Mapper(new MapperConfiguration(expr =>
        {
            expr.AddExpressionMapping();
            expr.CreateMap<UserDto, User>()
                .ConstructUsing((dto, _) =>
                {
                    var user = _context.Users.Include(user => user.CurrentInstagram).Include(user => user.CurrentWork)
                        .FirstOrDefault(o => o.Id == dto.Id);
                    if (user != null) return user;
                    user = new User {Id = dto.Id};
                    _context.Users.Add(user);
                    return user;
                });
            expr.CreateMap<InstagramLiteDto, Instagram>()
                .ConstructUsing((dto, _) =>
                {
                    if (dto.Id != 0)
                        return _context.Instagrams.First(o => o.Id == dto.Id);

                    var detail = new Instagram();
                    _context.Instagrams.Add(detail);
                    return detail;
                });
            expr.CreateMap<WorkLiteDto, Work>()
                .ConstructUsing((dto, _) =>
                {
                    if (dto.Id != 0)
                        return _context.Works.First(o => o.Id == dto.Id);

                    var detail = new Work();
                    _context.Works.Add(detail);
                    return detail;
                });

            expr.CreateMap<User, UserDto>();
            expr.CreateMap<User, UserLiteDto>();
            expr.CreateMap<Work, WorkLiteDto>();
            expr.CreateMap<Instagram, InstagramLiteDto>();
        }));
    }
}