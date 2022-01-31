using AutoMapper;
using DirectBot.API.ViewModels;
using DirectBot.API.ViewModels.Proxy;
using DirectBot.API.ViewModels.Subscribe;
using DirectBot.API.ViewModels.User;
using DirectBot.Core.DTO;
using DirectBot.Core.Models;

namespace DirectBot.API.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ProxyDto, ProxyViewModel>();
        CreateMap<UserDto, UserViewModel>().ReverseMap();
        CreateMap<SubscribeDto, SubscribeViewModel>().ForMember(x => x.UserId,
            expression => expression.MapFrom((dto, _) => dto.User?.Id));


        CreateMap<SubscribeDto, AddSubscribeViewModel>().ForMember(x => x.UserId,
            expression => expression.MapFrom((dto, _) => dto.User?.Id));
        CreateMap<AddSubscribeViewModel, SubscribeDto>().ForMember(x => x.User,
            expression => expression.MapFrom((dto, _) => new UserDto {Id = dto.UserId}));


        CreateMap<UserSearchQuery, UserSearchViewModel>().ReverseMap();
        CreateMap<ProxySearchQuery, ProxySearchViewModel>().ReverseMap();
        CreateMap<SubscribeSearchQuery, SubscribeSearchViewModel>().ReverseMap();
    }
}