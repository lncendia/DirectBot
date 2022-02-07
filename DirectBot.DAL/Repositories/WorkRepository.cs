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
        _context.Works.Include(work => work.User).Include(work => work.Instagrams)
            .ThenInclude(instagram => instagram.Proxy)
            .ProjectTo<WorkDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(work => work.Id == id);

    public async Task UpdateWithoutStatusAsync(WorkDto entity)
    {
        var u = await _context.Works.Persist(_mapper).InsertOrUpdateAsync(entity);
        var entry = _context.Entry(u);
        entry.Property(w => w.IsCompleted).IsModified = false;
        entry.Property(w => w.IsSucceeded).IsModified = false;
        entry.Property(w => w.IsCanceled).IsModified = false;
        await _context.SaveChangesAsync();
    }

    public Task<WorkDto?> GetUserWorksAsync(UserDto userDto, int page) =>
        _context.Works.Where(work => work.User.Id == userDto.Id).Include(work => work.Instagrams)
            .OrderByDescending(work => work.Id).Skip(page - 1)
            .ProjectTo<WorkDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();

    public Task<bool> HasActiveWorksAsync(InstagramDto instagram) =>
        _context.Works.AnyAsync(work =>
            work.Instagrams.Any(instagram1 => instagram1.Id == instagram.Id) && !work.IsCompleted);

    public Task<bool> IsCancelled(WorkDto workDto) =>
        _context.Works.Select(work => work.IsCanceled).FirstOrDefaultAsync();

    public async Task AddInstagramToWork(WorkDto workDto, InstagramDto instagramDto)
    {
        var work = await _context.Works.Persist(_mapper).InsertOrUpdateAsync(workDto);
        var instagram = await _context.Instagrams.Persist(_mapper).InsertOrUpdateAsync(instagramDto);
        work.Instagrams.Add(instagram);
        await _context.SaveChangesAsync();
    }
}