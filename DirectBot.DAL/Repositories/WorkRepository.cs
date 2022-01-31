using AutoMapper;
using AutoMapper.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using DirectBot.Core.Models;
using DirectBot.Core.Repositories;
using DirectBot.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace DirectBot.DAL.Repositories;

public class WorkRepository : IWorkRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public WorkRepository(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public Task<List<WorkDto>> GetAllAsync() =>
        _context.Works.ProjectTo<WorkDto>(_mapper.ConfigurationProvider).ToListAsync();

    public async Task AddOrUpdateAsync(WorkDto entity)
    {
        var u = await _context.Works.Persist(_mapper).InsertOrUpdateAsync(entity);
        await _context.SaveChangesAsync();
        entity.Id = u.Id;
    }

    public async Task DeleteAsync(WorkDto entity)
    {
        await _context.Works.Persist(_mapper).RemoveAsync(entity);
        await _context.SaveChangesAsync();
    }

    public Task<WorkDto?> GetAsync(int id) =>
        _context.Works.Include(work => work.Instagram.User).Include(work => work.Instagram.Proxy)
            .ProjectTo<WorkDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(work => work.Id == id);

    public Task<WorkDto?> GetUserWorksAsync(UserDto userDto, int page) =>
        _context.Works.Where(work => work.Instagram.UserId == userDto.Id).OrderByDescending(work => work.StartTime)
            .Skip(page - 1)
            .ProjectTo<WorkDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();

    public Task<int> GetInstagramWorksCountAsync(InstagramDto instagram) =>
        _context.Works.Where(work => work.Instagram.Id == instagram.Id).CountAsync();

    public Task<bool> HasActiveWorksAsync(InstagramDto instagram) =>
        _context.Works.AnyAsync(work => work.Instagram.Id == instagram.Id && !work.IsCompleted);

    public Task<List<WorkDto>> GetUserActiveWorksAsync(UserDto userDto) =>
        _context.Works
            .Where(work => work.Instagram.UserId == userDto.Id && !work.IsCompleted && string.IsNullOrEmpty(work.JobId))
            .ProjectTo<WorkDto>(_mapper.ConfigurationProvider).ToListAsync();
}