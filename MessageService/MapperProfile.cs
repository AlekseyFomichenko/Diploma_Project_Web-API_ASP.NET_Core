using AutoMapper;
using MessageService.Models.Dto;
using MessageService.Models;

namespace MessageService
{
    public class MapperProfiles : Profile
    {
        public MapperProfiles()
        {
            CreateMap<Message, MessageDTO>(MemberList.Destination).ReverseMap();
        }
    }
}
