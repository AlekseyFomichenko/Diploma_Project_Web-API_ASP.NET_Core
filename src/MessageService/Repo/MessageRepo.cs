using AutoMapper;
using MessageService.Db;
using MessageService.Interfaces;
using MessageService.Models;
using MessageService.Models.Dto;

namespace MessageService.Repo
{
    public sealed class MessageRepo : IMessageRepo
    {
        private readonly MessageContext _db;
        private readonly IMapper _mapper;

        public MessageRepo(MessageContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public List<MessageDTO> GetAllMessages(int receiverId)
        {
            var messages = _db.Messages.Where(x => x.ReceiverId == receiverId && !x.IsRead).ToList();
            foreach (var x in messages)
                x.IsRead = true;
            _db.SaveChanges();
            return messages.Select(_mapper.Map<MessageDTO>).ToList();
        }

        public int SendMessage(string text, int senderId, int receiverId)
        {
            _db.Messages.Add(new Message { Text = text, SenderId = senderId, ReceiverId = receiverId, IsRead = false });
            _db.SaveChanges();
            return 1;
        }
    }
}
