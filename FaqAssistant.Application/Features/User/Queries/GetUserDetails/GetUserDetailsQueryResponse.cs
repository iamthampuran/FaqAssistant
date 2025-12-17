namespace FaqAssistant.Application.Features.User.Queries.GetUserDetails;

public record GetUserDetailsQueryResponse(Guid Id, string Username, string Email, DateTime CreatedAt);
