namespace FaqAssistant.Application.Features.Tag.Queries.GetTagDetailsById
{
    public record GetTagDetailsByIdResponse (Guid Id, string Name, DateTime CreatedAt);
    
}
