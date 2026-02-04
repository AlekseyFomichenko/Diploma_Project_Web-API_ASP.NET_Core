using System.ComponentModel.DataAnnotations.Schema;

namespace MessageService.Models
{
    [Table("messages")]
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Text { get; set; } = null!;
        public bool IsRead { get; set; }
    }
}
