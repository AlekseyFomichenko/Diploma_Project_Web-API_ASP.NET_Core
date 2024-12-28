using AutoMapper;
using MessageService.Models.Dto;
using MessageService.Models;

namespace MessageService
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Message, MessageDTO>(MemberList.Destination).ReverseMap();
        }
    }
}
