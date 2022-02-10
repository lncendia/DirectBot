using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Repositories;

public interface IPaymentRepository : IRepository<PaymentDto, string>
{
    Task<List<PaymentDto>> GetAllAsync();
    Task<List<PaymentDto>> GetUserPaymentsAsync(UserLiteDto user, int page);
}