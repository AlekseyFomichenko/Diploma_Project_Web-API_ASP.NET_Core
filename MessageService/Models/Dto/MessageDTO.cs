namespace MessageService.Models.Dto
{
    public class MessageDTO
    {
        public string SenderMail { get; set; }
        public string ReceiverMail { get; set; }
        public string Text { get; set; }
        public bool IsRead { get; set; }
    }
}
