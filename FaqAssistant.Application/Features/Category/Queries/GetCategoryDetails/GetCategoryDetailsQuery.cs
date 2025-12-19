using FaqAssistant.Application.Common;
using FaqAssistant.Application.Helpers;
using MediatR;

namespace FaqAssistant.Application.Features.Category.Queries.GetCategoryDetails;

public record GetCategoryDetailsQuery(int PageSize, int PageCount, string? SearchValue) : IRequest<Result<PagedResult<GetCategoryDetailsResponse>>>;
