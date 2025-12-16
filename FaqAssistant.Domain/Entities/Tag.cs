namespace FaqAssistant.Domain.Entities
{
    public class Tag
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public IEnumerable<Faq> Faqs { get; private set; } = null!;
    }
}
