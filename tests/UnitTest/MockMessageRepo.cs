using MessageService.Interfaces;
using MessageService.Models.Dto;

namespace UnitTest
{
    public sealed class MockMessageRepo : IMessageRepo
    {
        public readonly List<MessageDTO> Messages = new();

        public MockMessageRepo()
        {
            Messages.Add(new MessageDTO { Text = "Hello", SenderId = 1, ReceiverId = 2 });
            Messages.Add(new MessageDTO { Text = "World", SenderId = 2, ReceiverId = 1 });
        }

        public int SendMessage(string text, int senderId, int receiverId)
        {
            Messages.Add(new MessageDTO { Text = text, SenderId = senderId, ReceiverId = receiverId, IsRead = false });
            return Messages.Count;
        }

        public List<MessageDTO> GetAllMessages(int receiverId)
        {
            return Messages.Where(m => m.ReceiverId == receiverId).ToList();
        }
    }
}
