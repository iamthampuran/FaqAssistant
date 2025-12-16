namespace FaqAssistant.Domain.Entities
{
    public class Answer
    {
        public Guid Id { get; private set; }
        public string Content { get; private set; } = null!;
        public Guid FaqId { get; private set; }
        public Guid UserId { get; private set; }
        public int Rating { get; private set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
