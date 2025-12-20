using FaqAssistant.Application.Common;
using FaqAssistant.Application.Helpers;
using MediatR;

namespace FaqAssistant.Application.Features.User.Queries.GetUserDetails;

public record GetUserDetailsQuery(int PageSize, int PageCount, string? SearchValue) : IRequest<Result<PagedResult<GetUserDetailsResponse>>>;
