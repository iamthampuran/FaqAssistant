namespace FaqAssistant.Application.Features.Tag.Queries.GetTagDetails
{
    public record GetTagDetailsQueryResponse(Guid Id, string Name, DateTime CreatedAt);
}
