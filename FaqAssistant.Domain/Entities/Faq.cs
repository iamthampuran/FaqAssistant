namespace FaqAssistant.Domain.Entities
{
    public class Faq
    {
        public Guid Id { get; private set; }
        public string Question { get; private set; } = null!;
        public Guid CategoryId { get; private set; }
        public Guid UserId { get; private set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public Category Category { get; private set; } = null!;
        public User User { get; private set; } = null!;
        public IEnumerable<Answer> Answers { get; private set; } = null!;
        public IEnumerable<Tag> Tags { get; private set; } = null!;

    }
}
