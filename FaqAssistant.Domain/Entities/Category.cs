namespace FaqAssistant.Domain.Entities
{
    public class Category : EntityBase
    {
        public string Name { get; private set; } = null!;
        public IEnumerable<Faq> Faqs { get; private set; } = null!;
    }
}
