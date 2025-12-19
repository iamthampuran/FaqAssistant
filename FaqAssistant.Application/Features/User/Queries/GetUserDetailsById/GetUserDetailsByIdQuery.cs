using FaqAssistant.Application.Helpers;
using MediatR;

namespace FaqAssistant.Application.Features.User.Queries.GetUserDetailsById;

public  record GetUserDetailsByIdQuery(Guid UserId) : IRequest<Result<GetUserDetailsByIdQueryResponse>>;
