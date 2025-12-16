namespace FaqAssistant.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Username { get; private set; } = null!;
        public string Email { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public IEnumerable<Faq> Faqs { get; private set; } = null!;
        public IEnumerable<Answer> Answers { get; private set; } = null!;
    }
}
