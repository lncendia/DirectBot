using DirectBot.Core.DTO;
using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IBillService
{
    public Task<IResult<Payment>> CreateBillAsync(UserDto user, int countSubscribes);
    public Task<IOperationResult> CheckPaymentAsync(string id);
}