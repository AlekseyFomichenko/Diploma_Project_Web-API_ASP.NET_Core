using MessageService.Interfaces;
using MessageService.Models.Dto;

namespace UnitTest
{
    public class MockMessageRepo : IMessageRepo
    {
        private readonly List<MessageDTO> _messages;

        public MockMessageRepo()
        {
            _messages = new List<MessageDTO>
        {
            new MessageDTO { Text = "Hello", SenderMail = "123@example.com", ReceiverMail = "321@example.com" },
            new MessageDTO { Text = "World", SenderMail = "321@example.com", ReceiverMail = "123@example.com" }
        };
        }
        public int SendMessage(string text, string userFrom, string userTo)
        {
            throw new NotImplementedException();
        }

        public List<MessageDTO> GetAllMessages(string userTo)
        {
            return _messages.Where(m => m.ReceiverMail == userTo).ToList();
        }
    }
}
