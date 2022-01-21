using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;
using DirectBot.Core.Repositories;
using DirectBot.Core.Services;

namespace DirectBot.BLL.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;

    public PaymentService(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }


    public Task<PaymentDto?> GetAsync(string id)
    {
        return _paymentRepository.GetAsync(id);
    }

    public async Task<IOperationResult> UpdateAsync(PaymentDto entity)
    {
        try
        {
            await _paymentRepository.AddOrUpdateAsync(entity);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }

    public async Task<IOperationResult> AddAsync(PaymentDto item)
    {
        try
        {
            await _paymentRepository.AddOrUpdateAsync(item);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }

    public Task<List<PaymentDto>> GetUserPaymentsAsync(UserDto user, int page)
    {
        return _paymentRepository.GetUserPaymentsAsync(user, page);
    }

    public Task<List<PaymentDto>> GetAllAsync()
    {
        return _paymentRepository.GetAllAsync();
    }

    public async Task<IOperationResult> DeleteAsync(PaymentDto payment)
    {
        try
        {
            await _paymentRepository.DeleteAsync(payment);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }
}
