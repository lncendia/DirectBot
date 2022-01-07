using System.Net;
using Newtonsoft.Json;

namespace DirectBot.BLL.Services;

public class PaymentService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public PaymentService(ApplicationDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public List<Payment> GetPayments(User user, int page)
    {
        return _dbContext.Payments.Include(payment => payment.Rate).Where(payment => payment.User == user)
            .Skip((page - 1) * 30).Take(30)
            .OrderByDescending(payment => payment.CreationDate).ToList();
    }

    public List<Rate> GetRates()
    {
        return _dbContext.Rates.AsEnumerable().OrderBy(rate => rate.Amount).ToList();
    }

    public int GetPaymentsCount(User user)
    {
        return _dbContext.Payments.Count(payment => payment.User == user);
    }

    public bool DeleteRate(Rate rate)
    {
        try
        {
            var newRate = _dbContext.Rates.Where(rate1 => rate1 != rate).AsEnumerable()
                .OrderBy(rate1 => rate1.Amount).FirstOrDefault();
            if (newRate != null)
            {
                rate.Users.ForEach(user =>
                {
                    user.EndOfSubscribe = RateRecalculation(user, newRate);
                    user.Rate = newRate;
                });
            }

            _dbContext.RemoveRange(rate.Payments);
            _dbContext.Remove(rate);
            _dbContext.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private DateTime RateRecalculation(User user, Rate rate)
    {
        if (user.Rate == null || user.EndOfSubscribe < DateTime.Now) return DateTime.Now;
        var rest = (user.EndOfSubscribe - DateTime.Today).Days * user.Rate.Amount / user.Rate.Duration;
        int duration = (int) (rate.Duration * rest / rate.Amount);
        return DateTime.Now.AddDays(duration);
    }


    public Payment CreateBill(User user, Rate rate)
    {
        try
        {
            BillPaymentsClient client = BillPaymentsClientFactory.Create(_configuration["Payments:SecretKey"]);
            var response = client.CreateBill(
                new CreateBillInfo
                {
                    BillId = Guid.NewGuid().ToString(),
                    Amount = new MoneyAmount
                    {
                        ValueDecimal = rate.Amount,
                        CurrencyEnum = CurrencyEnum.Rub
                    },
                    ExpirationDateTime = DateTime.Now.AddDays(5),
                    Customer = new Customer
                    {
                        Account = user.Id
                    }
                });

            var payment = new Payment
            {
                User = user, CreationDate = response.CreationDateTime,
                PayUrl = response.PayUrl.ToString(), Id = response.BillId, Rate = rate
            };
            _dbContext.Add(payment);
            _dbContext.SaveChanges();
            return payment;
        }
        catch
        {
            return null;
        }
    }

    public bool CheckPayment(Payment payment)
    {
        try
        {
            RestClient httpClient = new()
            {
                BaseUrl = new Uri($"https://api.qiwi.com/partner/bill/v1/bills/{payment.Id}")
            };
            var request = new RestRequest(Method.GET);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Authorization", $"Bearer {_configuration["Payments:SecretKey"]}");
            IRestResponse response1 = httpClient.Execute(request);
            if (response1.StatusCode != HttpStatusCode.OK) return false;
            var jObject = JsonConvert.DeserializeObject<BillResponse>(response1.Content);
            if (jObject?.Status.ValueString != "PAID") return false;
            payment.User.EndOfSubscribe = payment.Rate == payment.User.Rate
                ? payment.User.EndOfSubscribe.AddDays(payment.Rate.Duration)
                : RateRecalculation(payment.User, payment.Rate).AddDays(payment.Rate.Duration);
            payment.User.Rate = payment.Rate;
            payment.PaymentDate = jObject.Status.ChangedDateTime;
            payment.Success = true;
            _dbContext.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }
}