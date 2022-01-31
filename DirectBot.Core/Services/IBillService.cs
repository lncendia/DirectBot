using DirectBot.Core.DTO;
using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IBillService
{
    Task<IResult<Payment>> CreateBillAsync(UserDto user, int countSubscribes);
    Task<IResult<PaymentDto>> GetPaymentAsync(string id);
}