namespace FaqAssistant.Application.Features.User.Queries.GetUserDetails;

public record GetUserDetailsResponse(Guid Id, string Username, string Email, DateTime CreatedAt);
