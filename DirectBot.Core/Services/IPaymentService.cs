using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IPaymentService : IService<PaymentDto, string>
{
    Task<List<PaymentDto>> GetUserPaymentsAsync(long id, int page);
}