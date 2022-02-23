using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using AutoMapper.QueryableExtensions;
using DirectBot.Core.Models;
using DirectBot.Core.Repositories;
using DirectBot.DAL.Data;
using DirectBot.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DirectBot.DAL.Repositories;

public class WorkRepository : IWorkRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public WorkRepository(ApplicationDbContext context)
    {
        _context = context;
        _mapper = GetMapper();
    }

    public async Task AddOrUpdateAsync(WorkDto entity)
    {
        var u = _mapper.Map<Work>(entity);
        await _context.SaveChangesAsync();
        entity.Id = u.Id;
    }

    public async Task DeleteAsync(WorkDto entity)
    {
        var u = _mapper.Map<Work>(entity);
        _context.Remove(u);
        await _context.SaveChangesAsync();
    }

    public Task<WorkDto?> GetAsync(int id) =>
        _context.Works.ProjectTo<WorkDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(work => work.Id == id);

    public async Task UpdateProcessingInfoAsync(WorkDto entity)
    {
        var u = _mapper.Map<Work>(entity);
        var entry = _context.Entry(u);
        entry.Property(w => w.JobId).IsModified = false;
        entry.Property(w => w.StartTime).IsModified = false;
        entry.Property(w => w.IsCompleted).IsModified = false;
        await _context.SaveChangesAsync();
    }

    public Task<WorkDto?> GetUserWorksAsync(long id, int page) =>
        _context.Works.Where(work => work.Instagrams.Any(instagram => instagram.User.Id == id))
            .OrderByDescending(work => work.Id).Skip(page - 1)
            .ProjectTo<WorkDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();

    public Task<bool> HasActiveWorksAsync(int instagramId) =>
        _context.Works.AnyAsync(work =>
            work.Instagrams.Any(instagram1 => instagram1.Id == instagramId) && !work.IsCompleted);

    public Task<int> GetInstagramsCountAsync(int id) =>
        _context.Works.Where(work => work.Id == id).Select(work => work.Instagrams.Count).FirstOrDefaultAsync();


    public Task<List<WorkDto>> GetExpiredWorks()
    {
        return _context.Works.Where(work => DateTime.Now.AddDays(-15) >= work.StartTime)
            .ProjectTo<WorkDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    private IMapper GetMapper()
    {
        return new Mapper(new MapperConfiguration(expr =>
        {
            expr.AddExpressionMapping();
            expr.CreateMap<WorkDto, Work>()
                .ConstructUsing((dto, _) =>
                {
                    if (dto.Id != 0)
                        return _context.Works.Include(work => work.Instagrams).First(o => o.Id == dto.Id);

                    var work = new Work();
                    _context.Works.Add(work);
                    return work;
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

            expr.CreateMap<Work, WorkDto>();
            expr.CreateMap<Work, WorkLiteDto>();
            expr.CreateMap<Instagram, InstagramLiteDto>();
        }));
    }
}