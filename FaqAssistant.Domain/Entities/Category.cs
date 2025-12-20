namespace FaqAssistant.Domain.Entities;

public class Category : EntityBase
{
    public string Name { get; set; } = null!;
    public virtual IEnumerable<Faq> Faqs { get; set; } = null!;
}
