using AutoMapper;
using DirectBot.Core.Configuration;
using DirectBot.Core.Models;
using DirectBot.Core.Services;

namespace DirectBot.BLL;

public class ServiceContainer
{
    public readonly IInstagramLoginService InstagramLoginService;
    public readonly IUserService UserService;
    public readonly IBillService BillService;
    public readonly ISubscribeService SubscribeService;
    public readonly IWorkService WorkService;
    public readonly IWorkerService WorkerService;
    public readonly Configuration Configuration;
    public readonly IInstagramService InstagramService;
    public readonly IPaymentService PaymentService;
    public readonly IMapper Mapper;

    public ServiceContainer(IUserService userService, IWorkerService workerService, ISubscribeService subscribeService,
        IBillService billService, IInstagramLoginService instagramLoginService,
        IInstagramService instagramService, IWorkService workService, IPaymentService paymentService,
        Configuration configuration)
    {
        UserService = userService;
        WorkerService = workerService;
        SubscribeService = subscribeService;
        BillService = billService;
        InstagramLoginService = instagramLoginService;
        Configuration = configuration;
        InstagramService = instagramService;
        WorkService = workService;
        PaymentService = paymentService;
        Mapper = GetMapper();
    }

    private static IMapper GetMapper()
    {
        return new Mapper(new MapperConfiguration(expr =>
        {
            expr.CreateMap<WorkDto, WorkLiteDto>();
            expr.CreateMap<InstagramDto, InstagramLiteDto>();
            expr.CreateMap<UserDto, UserLiteDto>();
        }));
    }
}