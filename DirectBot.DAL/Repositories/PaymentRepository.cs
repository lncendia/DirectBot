using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using AutoMapper.QueryableExtensions;
using DirectBot.Core.Models;
using DirectBot.Core.Repositories;
using DirectBot.DAL.Data;
using DirectBot.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DirectBot.DAL.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public PaymentRepository(ApplicationDbContext context)
    {
        _context = context;
        _mapper = GetMapper();
    }

    public async Task AddOrUpdateAsync(PaymentDto entity)
    {
        var u = _mapper.Map<Payment>(entity);
        await _context.SaveChangesAsync();
        entity.Id = u.Id;
    }

    public async Task DeleteAsync(PaymentDto entity)
    {
        var u = _mapper.Map<Payment>(entity);
        _context.Remove(u);
        await _context.SaveChangesAsync();
    }

    public Task<PaymentDto?> GetAsync(string id) =>
        _context.Payments.Include(payment => payment.User).ProjectTo<PaymentDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(payment => payment.Id == id);

    public Task<List<PaymentDto>> GetUserPaymentsAsync(long id, int page) =>
        _context.Payments.Where(payment => payment.User.Id == id)
            .OrderByDescending(payment => payment.PaymentDate)
            .Skip((page - 1) * 5).Take(5).ProjectTo<PaymentDto>(_mapper.ConfigurationProvider).ToListAsync();

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
            expr.CreateMap<PaymentDto, Payment>()
                .ConstructUsing((dto, _) =>
                {
                    if (string.IsNullOrEmpty(dto.Id))
                        return _context.Payments.Include(payment => payment.User).First(o => o.Id == dto.Id);

                    var detail = new Payment();
                    _context.Payments.Add(detail);
                    return detail;
                });

            expr.CreateMap<User, UserLiteDto>();
            expr.CreateMap<Payment, PaymentDto>();
        }));
    }
}