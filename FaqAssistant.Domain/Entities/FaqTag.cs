namespace FaqAssistant.Domain.Entities;

public class FaqTag : EntityBase
{
    public Guid FaqId { get; set; }
    public virtual Faq Faq { get; set; } = null!;

    public Guid TagId { get; set; }
    public virtual Tag Tag { get; set; } = null!;
}

