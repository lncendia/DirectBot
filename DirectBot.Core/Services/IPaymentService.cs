using DirectBot.Core.DTO;
using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IPaymentService : IService<PaymentDto, string>
{
    Task<List<PaymentDto>> GetUserPaymentsAsync(UserDto user, int page);
}