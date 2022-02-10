using AutoMapper;
using DirectBot.DAL.Models;
using DirectBot.Core.Models;

namespace DirectBot.DAL.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserDto, User>().ForMember(user => user.CurrentInstagramId,
                expression => expression.MapFrom((dto, _) => dto.CurrentInstagram?.Id))
            .ForMember(user => user.CurrentInstagram,
                expression => expression.Ignore())
            .ForMember(user => user.CurrentInstagram,
                expression => expression.MapFrom((dto, _) => dto.CurrentInstagram?.Id))
            .ForMember(user => user.CurrentWork,
                expression => expression.Ignore());
        CreateMap<User, UserDto>();
        CreateMap<User, UserLiteDto>();


        CreateMap<InstagramDto, Instagram>()
            .ForMember(x => x.UserId,
                expression => expression.MapFrom((dto, _) => dto.User?.Id))
            .ForMember(x => x.User, expression => expression.Ignore())
            .ForMember(x => x.Proxy,
                expression => expression.MapFrom((dto, _) => dto.Proxy?.Id))
            .ForMember(x => x.Proxy, expression => expression.Ignore());

        CreateMap<Instagram, InstagramDto>();
        CreateMap<Instagram, InstagramLiteDto>();


        CreateMap<ProxyDto, Proxy>().ForMember(x => x.Instagrams, expression => expression.Ignore());
        CreateMap<Proxy, ProxyDto>();

        CreateMap<SubscribeDto, Subscribe>()
            .ForMember(x => x.UserId,
                expression => expression.MapFrom((dto, _) => dto.User?.Id))
            .ForMember(x => x.User, expression => expression.Ignore());
        CreateMap<Subscribe, SubscribeDto>();


        CreateMap<WorkDto, Work>()
            .ForMember(x => x.UserId,
                expression => expression.MapFrom((dto, _) => dto.User?.Id))
            .ForMember(x => x.Instagrams, expression => expression.Ignore())
            .ForMember(x => x.User, expression => expression.Ignore());
        CreateMap<Work, WorkDto>();
        CreateMap<Work, WorkLiteDto>();


        CreateMap<PaymentDto, Payment>()
            .ForMember(x => x.UserId,
                expression => expression.MapFrom((dto, _) => dto.User?.Id))
            .ForMember(x => x.User, expression => expression.Ignore());
        CreateMap<Payment, PaymentDto>();
    }
}