using MessageService.Models.Dto;

namespace MessageService.Interfaces
{
    public interface IMessageRepo
    {
        int SendMessage(string text, int senderId, int receiverId);
        List<MessageDTO> GetAllMessages(int receiverId);
    }
}
