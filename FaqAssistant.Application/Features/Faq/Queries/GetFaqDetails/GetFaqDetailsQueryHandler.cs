using FaqAssistant.Application.Common;
using FaqAssistant.Application.Helpers;
using FaqAssistant.Application.Interfaces.Repositories;
using MediatR;

namespace FaqAssistant.Application.Features.Faq.Queries.GetFaqDetails;

public class GetFaqDetailsQueryHandler : IRequestHandler<GetFaqDetailsQuery, Result<PagedResult<GetFaqDetailsResponse>>>
{
    private readonly IFaqRepository _faqRepository;
    public GetFaqDetailsQueryHandler(IFaqRepository faqRepository)
    {
        _faqRepository = faqRepository ?? throw new ArgumentNullException(nameof(faqRepository));
    }
    public async Task<Result<PagedResult<GetFaqDetailsResponse>>> Handle(GetFaqDetailsQuery request, CancellationToken cancellationToken)
    {
        var faqDetails = await _faqRepository.GetFaqDetailsAsync(request.PageParameters, request.CategoryId, request.TagId);
        return new Result<PagedResult<GetFaqDetailsResponse>>(true, faqDetails);
    }
}
