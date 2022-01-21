using AutoMapper;
using DirectBot.DAL.Models;
using DirectBot.Core.Models;

namespace DirectBot.DAL.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserDto, User>().ReverseMap();


        CreateMap<InstagramDto, Instagram>()
            .ForMember(x => x.UserId,
                expression => expression.MapFrom((dto,_) => dto.User?.Id))
            .ForMember(x => x.User, expression => expression.Ignore())
            .ForMember(x => x.Proxy,
                expression => expression.MapFrom((dto, _) => dto.Proxy?.Id))
            .ForMember(x => x.Proxy, expression => expression.Ignore());

        CreateMap<Instagram, InstagramDto>();


        CreateMap<ProxyDto, Proxy>().ReverseMap();


        CreateMap<SubscribeDto, Subscribe>()
            .ForMember(x => x.UserId,
                expression => expression.MapFrom((dto,_) => dto.User?.Id))
            .ForMember(x => x.User, expression => expression.Ignore());
        CreateMap<Subscribe, SubscribeDto>();


        CreateMap<WorkDto, Work>()
            .ForMember(x => x.InstagramId,
                expression => expression.MapFrom((dto,_) => dto.Instagram?.Id))
            .ForMember(x => x.Instagram, expression => expression.Ignore());
        CreateMap<Work, WorkDto>();


        CreateMap<PaymentDto, Payment>()
            .ForMember(x => x.UserId,
                expression => expression.MapFrom((dto,_) => dto.User?.Id))
            .ForMember(x => x.User, expression => expression.Ignore());
        CreateMap<Payment, PaymentDto>();
    }
}