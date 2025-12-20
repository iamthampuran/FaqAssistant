using FaqAssistant.Application.Common;
using FaqAssistant.Application.Helpers;
using MediatR;

namespace FaqAssistant.Application.Features.Faq.Queries.GetFaqDetails;

public record GetFaqDetailsQuery(PageParameters PageParameters, Guid? CategoryId, Guid? TagId) : IRequest<Result<PagedResult<GetFaqDetailsResponse>>>;
