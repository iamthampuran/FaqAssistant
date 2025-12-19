namespace FaqAssistant.Application.Features.User.Queries.GetUserDetailsById;

public record GetUserDetailsByIdResponse(Guid UserId, string UserName, string Email);

