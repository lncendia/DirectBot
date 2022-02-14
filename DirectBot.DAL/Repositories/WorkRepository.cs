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

    public Task<List<WorkLiteDto>> GetAllAsync() =>
        _context.Works.ProjectTo<WorkLiteDto>(_mapper.ConfigurationProvider).ToListAsync();

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
        _context.Works.ProjectTo<WorkDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(work => work.Id == id);

    public async Task UpdateProcessingInfoAsync(WorkDto entity)
    {
        var u = await _context.Works.Persist(_mapper).InsertOrUpdateAsync(entity);
        var entry = _context.Entry(u);
        entry.Property(w => w.JobId).IsModified = false;
        entry.Property(w => w.StartTime).IsModified = false;
        entry.Property(w => w.IsCompleted).IsModified = false;
        entry.Property(w => w.IsSucceeded).IsModified = false;
        entry.Property(w => w.IsCanceled).IsModified = false;
        await _context.SaveChangesAsync();
    }

    public Task<WorkDto?> GetUserWorksAsync(UserLiteDto userDto, int page) =>
        _context.Works.Where(work => work.Instagrams.Any(instagram => instagram.User.Id == userDto.Id))
            .OrderByDescending(work => work.Id).Skip(page - 1)
            .ProjectTo<WorkDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();

    public Task<bool> HasActiveWorksAsync(InstagramLiteDto instagram) =>
        _context.Works.AnyAsync(work =>
            work.Instagrams.Any(instagram1 => instagram1.Id == instagram.Id) && !work.IsCompleted);

    public Task<bool> IsCancelled(WorkLiteDto workDto) =>
        _context.Works.Where(work => work.Id == workDto.Id).Select(work => work.IsCanceled).FirstOrDefaultAsync();

    public async Task AddInstagramToWork(WorkLiteDto workDto, InstagramLiteDto instagramDto)
    {
        var work = await _context.Works.FirstOrDefaultAsync(work1 => work1.Id == workDto.Id);
        var instagram = await _context.Instagrams.FirstOrDefaultAsync(instagram1 => instagram1.Id == instagramDto.Id);
        if (work == null || instagram == null) throw new ArgumentException("Не удалось найти указанные сущности");
        work.Instagrams.Add(instagram);
        await _context.SaveChangesAsync();
    }

    public Task<List<WorkDto>> GetExpiredSubscribes()
    {
        return _context.Works.Where(work => DateTime.Now.AddDays(-15) >= work.StartTime)
            .ProjectTo<WorkDto>(_mapper.ConfigurationProvider).ToListAsync();
    }
}