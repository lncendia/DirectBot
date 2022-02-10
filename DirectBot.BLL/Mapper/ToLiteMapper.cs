using AutoMapper;
using DirectBot.Core.Models;

namespace DirectBot.BLL.Mapper;

public sealed class ToLiteMapper : Profile
{
    public ToLiteMapper()
    {
        CreateMap<UserDto, UserLiteDto>();
        CreateMap<InstagramDto, InstagramLiteDto>();
        CreateMap<WorkDto, WorkLiteDto>();
    }
}