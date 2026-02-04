namespace MessageService.Models.Dto
{
    public record MessageDTO
    {
        public int SenderId { get; init; }
        public int ReceiverId { get; init; }
        public string Text { get; init; } = null!;
        public bool IsRead { get; init; }
    }

    public record SendMessageRequest
    {
        public string Text { get; init; } = null!;
        public int ReceiverId { get; init; }
    }
}
