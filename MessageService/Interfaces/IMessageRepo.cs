using MessageService.Models.Dto;

namespace MessageService.Interfaces
{
    public interface IMessageRepo
    {
        int SendMessage(string text, string userFrom, string userTo);
        List<MessageDTO> GetAllMessages(string userTo);
    }
}
