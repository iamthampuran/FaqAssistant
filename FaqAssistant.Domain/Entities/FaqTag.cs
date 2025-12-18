namespace FaqAssistant.Domain.Entities;

public class FaqTag
{
    public Guid FaqId { get; set; }
    public Faq Faq { get; set; } = null!;

    public Guid TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}

