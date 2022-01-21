using DirectBot.Core.Configuration;
using DirectBot.Core.Services;

namespace DirectBot.BLL;

public class ServiceContainer
{
    public readonly IInstagramLoginService InstagramLoginService;
    public readonly IUserService UserService;
    public readonly IBillService BillService;
    public readonly IProxyService ProxyService;
    public readonly ISubscribeService SubscribeService;
    public readonly IWorkService WorkService;
    public readonly IWorkerService WorkerService;
    public readonly Configuration Configuration;
    public readonly IInstagramService InstagramService;
    public readonly IPaymentService PaymentService;

    public ServiceContainer(IUserService userService, IWorkerService workerService, ISubscribeService subscribeService,
        IProxyService proxyService, IBillService billService, IInstagramLoginService instagramLoginService,
        IInstagramService instagramService, IWorkService workService, IPaymentService paymentService,
        Configuration configuration)
    {
        UserService = userService;
        WorkerService = workerService;
        SubscribeService = subscribeService;
        ProxyService = proxyService;
        BillService = billService;
        InstagramLoginService = instagramLoginService;
        Configuration = configuration;
        InstagramService = instagramService;
        WorkService = workService;
        PaymentService = paymentService;
    }
}