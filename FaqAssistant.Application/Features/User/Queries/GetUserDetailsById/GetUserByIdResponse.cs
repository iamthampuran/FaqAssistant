namespace FaqAssistant.Application.Features.User.Queries.GetUserDetailsById;

public record GetUserByIdResponse(Guid UserId, string UserName, string Email);

