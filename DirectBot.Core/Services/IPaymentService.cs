using DirectBot.Core.DTO;
using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;

namespace DirectBot.Core.Services;

public interface IPaymentService
{
    public Task<IResult<Payment>> CreateBillAsync(UserDTO user, int countSubscribes);
    public Task<IOperationResult> CheckPaymentAsync(string id);
}