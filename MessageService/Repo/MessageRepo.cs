using AutoMapper;
using MessageService.Interfaces;
using MessageService.Models.Dto;

namespace MessageService.Repo
{
    public class MessageRepo : IMessageRepo
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public MessageRepo(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }

        public List<MessageDTO> GetAllMessages(string userTo)
        {
            throw new NotImplementedException();
        }

        public int SendMessage(string text, string userFrom, string userTo)
        {
            throw new NotImplementedException();
        }
    }
}
