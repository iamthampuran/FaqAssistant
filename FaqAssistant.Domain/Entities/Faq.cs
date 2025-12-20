namespace FaqAssistant.Domain.Entities;

public class Faq : EntityBase
{
    public string Question { get; set; } = null!;
    public Guid CategoryId { get; set; }
    public string Answer { get; set; } = null!;
    public int Rating { get; set; }
    public Guid UserId { get; set; }
    public virtual Category Category { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    public virtual ICollection<FaqTag> Tags { get; set; } = [];

}
