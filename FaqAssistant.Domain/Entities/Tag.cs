namespace FaqAssistant.Domain.Entities;

public class Tag : EntityBase
{
    public string Name { get; set; } = null!;
    public IEnumerable<Faq> Faqs { get;  set; } = null!;
}
