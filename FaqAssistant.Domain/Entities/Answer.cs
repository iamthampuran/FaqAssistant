namespace FaqAssistant.Domain.Entities
{
    public class Answer : EntityBase
    {
        public string Content { get; private set; } = null!;
        public Guid FaqId { get; private set; }
        public Guid UserId { get; private set; }
        public int Rating { get; private set; }
        
    }
}
