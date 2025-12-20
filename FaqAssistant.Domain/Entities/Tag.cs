namespace FaqAssistant.Domain.Entities;

public class Tag : EntityBase
{
    public string Name { get; set; } = null!;
    public virtual ICollection<FaqTag> Faqs { get;  set; } = [];
}
