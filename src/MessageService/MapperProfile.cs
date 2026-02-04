using AutoMapper;
using MessageService.Models.Dto;
using MessageService.Models;

namespace MessageService
{
    public sealed class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Message, MessageDTO>(MemberList.Destination).ReverseMap();
        }
    }
}
