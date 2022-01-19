using AutoMapper;
using DirectBot.DAL.Models;
using DirectBot.Core.Models;

namespace DirectBot.DAL.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserDTO, User>();
        CreateMap<User, UserDTO>();

        CreateMap<InstagramDTO, Instagram>();
        CreateMap<Instagram, InstagramDTO>();

        CreateMap<ProxyDTO, Proxy>();
        CreateMap<Proxy, ProxyDTO>();

        CreateMap<SubscribeDTO, Subscribe>();
        CreateMap<Subscribe, SubscribeDTO>();

        CreateMap<WorkDTO, Work>();
        CreateMap<Work, WorkDTO>();
    }
}