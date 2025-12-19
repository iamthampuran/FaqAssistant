namespace FaqAssistant.Domain.Entities;

public class User : EntityBase
{
    public string Username { get;  set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public virtual IEnumerable<Faq> Faqs { get; set; } = null!;
}
