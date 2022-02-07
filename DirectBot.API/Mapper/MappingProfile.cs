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
        CreateMap<UserDto, UserViewModel>().ForMember(model => model.CurrentInstagramId,
                x => x.MapFrom((dto, _) => dto.CurrentInstagram?.Id))
            .ForMember(model => model.CurrentWorkId,
                x => x.MapFrom((dto, _) => dto.CurrentWork?.Id));
        CreateMap<UserViewModel, UserDto>().ForMember(model => model.CurrentInstagram,
                x => x.MapFrom((dto, _) =>
                    dto.CurrentInstagramId == null ? null : new InstagramLiteDto {Id = dto.CurrentInstagramId.Value}))
            .ForMember(model => model.CurrentWork,
                x => x.MapFrom((dto, _) =>
                    dto.CurrentWorkId == null ? null : new WorkLiteDto {Id = dto.CurrentWorkId.Value}));

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