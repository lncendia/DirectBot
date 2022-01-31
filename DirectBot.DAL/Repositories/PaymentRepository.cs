using AutoMapper;
using AutoMapper.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using DirectBot.Core.Models;
using DirectBot.Core.Repositories;
using DirectBot.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace DirectBot.DAL.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public PaymentRepository(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public Task<List<PaymentDto>> GetAllAsync() =>
        _context.Payments.ProjectTo<PaymentDto>(_mapper.ConfigurationProvider).ToListAsync();

    public async Task AddOrUpdateAsync(PaymentDto entity)
    {
        var u = await _context.Payments.Persist(_mapper).InsertOrUpdateAsync(entity);
        await _context.SaveChangesAsync();
        entity.Id = u.Id;
    }

    public async Task DeleteAsync(PaymentDto entity)
    {
        await _context.Payments.Persist(_mapper).RemoveAsync(entity);
        await _context.SaveChangesAsync();
    }

    public Task<PaymentDto?> GetAsync(string id) =>
        _context.Payments.Include(payment => payment.User).ProjectTo<PaymentDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(payment => payment.Id == id);

    public Task<List<PaymentDto>> GetUserPaymentsAsync(UserDto user, int page) =>
        _context.Payments.Where(payment => payment.User.Id == user.Id)
            .OrderByDescending(payment => payment.PaymentDate)
            .Skip((page - 1) * 5).Take(5).ProjectTo<PaymentDto>(_mapper.ConfigurationProvider).ToListAsync();
}