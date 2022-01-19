using System.Net;
using DirectBot.Core.Configuration;
using DirectBot.Core.DTO;
using DirectBot.Core.Interfaces;
using DirectBot.Core.Models;
using DirectBot.Core.Services;
using Newtonsoft.Json;
using Qiwi.BillPayments.Client;
using Qiwi.BillPayments.Model;
using Qiwi.BillPayments.Model.In;
using Qiwi.BillPayments.Model.Out;
using RestSharp;

namespace DirectBot.BLL.Services;

public class PaymentService : IPaymentService
{
    private readonly Configuration _configuration;

    public PaymentService(Configuration configuration)
    {
        _configuration = configuration;
    }

    public async Task<IResult<Payment>> CreateBillAsync(UserDTO user, int countSubscribes)
    {
        try
        {
            BillPaymentsClient client = BillPaymentsClientFactory.Create(_configuration.PaymentToken);
            var response = await client.CreateBillAsync(
                new CreateBillInfo
                {
                    BillId = Guid.NewGuid().ToString(),
                    Amount = new MoneyAmount
                    {
                        ValueDecimal = _configuration.Cost * countSubscribes,
                        CurrencyEnum = CurrencyEnum.Rub
                    },
                    ExpirationDateTime = DateTime.Now.AddDays(5),
                    Customer = new Customer
                    {
                        Account = user.Id.ToString()
                    }
                });

            var payment = new Payment
            {
                PayUrl = response.PayUrl.ToString(), Id = response.BillId, Cost = response.Amount.ValueDecimal
            };
            return Result<Payment>.Ok(payment);
        }
        catch (Exception ex)
        {
            return Result<Payment>.Fail(ex.Message);
        }
    }

    public async Task<IOperationResult> CheckPaymentAsync(string id)
    {
        try
        {
            RestClient httpClient = new($"https://api.qiwi.com/partner/bill/v1/bills/{id}");
            var request = new RestRequest();
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Authorization", $"Bearer {_configuration.PaymentToken}");
            var response1 = await httpClient.ExecuteAsync(request);
            if (response1.StatusCode != HttpStatusCode.OK)
                return OperationResult.Fail($"Unexpected response: {response1.StatusCode}");
            var jObject = JsonConvert.DeserializeObject<BillResponse>(response1.Content!);
            return jObject?.Status.ValueString != "PAID"
                ? OperationResult.Fail("The bill has not been paid")
                : OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }
}