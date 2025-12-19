using FaqAssistant.Application.Common;
using FaqAssistant.Application.Helpers;
using MediatR;

namespace FaqAssistant.Application.Features.Tag.Queries.GetTagDetails;

public record GetTagDetailsQuery(int PageSize, int PageCount, string? SearchValue) : IRequest<Result<PagedResult<GetTagDetailsResponse>>>;
