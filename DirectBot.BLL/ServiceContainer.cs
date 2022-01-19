using DirectBot.Core.Configuration;
using DirectBot.Core.Services;

namespace DirectBot.BLL;

public class ServiceContainer
{
    public readonly IInstagramLoginService InstagramLoginService;
    public readonly IUserService UserService;
    public readonly IPaymentService PaymentService;
    public readonly IProxyService ProxyService;
    public readonly ISubscribeService SubscribeService;
    public readonly IWorkService WorkService;
    public readonly IWorkerService WorkerService;
    public readonly Configuration Configuration;
    public readonly IInstagramService InstagramService;

    public ServiceContainer(IUserService userService, IWorkerService workerService, ISubscribeService subscribeService,
        IProxyService proxyService, IPaymentService paymentService, IInstagramLoginService instagramLoginService,
        IInstagramService instagramService, IWorkService workService, Configuration configuration)
    {
        UserService = userService;
        WorkerService = workerService;
        SubscribeService = subscribeService;
        ProxyService = proxyService;
        PaymentService = paymentService;
        InstagramLoginService = instagramLoginService;
        Configuration = configuration;
        InstagramService = instagramService;
        WorkService = workService;
    }
}