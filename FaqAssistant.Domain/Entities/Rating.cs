namespace FaqAssistant.Domain.Entities;

public class Rating : EntityBase
{
    public Guid FaqId { get; set; }
    public Faq Faq { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public bool IsUpvote { get; set; }
}
