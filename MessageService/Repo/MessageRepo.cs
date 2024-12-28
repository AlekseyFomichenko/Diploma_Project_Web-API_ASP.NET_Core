using AutoMapper;
using MessageService.Db;
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
            using (var db = new MessageContext(_configuration.GetValue<string>("ConnectionStrings")))
            {
                var messages = db.Messages.Where(x => x.ReceiverMail == userTo && x.IsRead == false).ToList();
                messages.ForEach(x => { x.IsRead = true; });
                db.SaveChanges();
                return messages.Select(_mapper.Map<MessageDTO>).ToList();
            }
        }

        public int SendMessage(string text, string userFrom, string userTo)
        {
            using (var db = new MessageContext(_configuration.GetValue<string>("ConnectionStrings")))
            {
                bool isUserFromExists = db.Users.FirstOrDefault(x => x.Name == userFrom) != null;
                bool isUserToExists = db.Users.FirstOrDefault(x => x.Name == userTo) != null;
                if (!isUserFromExists && isUserToExists)
                    return -1;

                db.Messages.Add(new Models.Message() { IsRead = false, ReceiverMail = userTo, SenderMail = userFrom, Text = text });
                db.SaveChanges();
                return 1;
            }
        }
    }
}
