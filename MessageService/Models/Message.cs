﻿namespace MessageService.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string SenderMail { get; set; }
        public string ReceiverMail { get; set; }
        public string Text { get; set; }
        public bool IsRead { get; set; }
    }
}
