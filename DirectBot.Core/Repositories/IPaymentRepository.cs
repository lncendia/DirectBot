using DirectBot.Core.DTO;
using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Repositories;

public interface IPaymentRepository : IRepository<PaymentDto, string>
{
    Task<List<PaymentDto>> GetUserPaymentsAsync(UserDto user, int page);
}