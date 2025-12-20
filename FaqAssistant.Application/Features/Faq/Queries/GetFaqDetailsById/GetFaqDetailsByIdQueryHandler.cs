using FaqAssistant.Application.Helpers;
using FaqAssistant.Application.Interfaces.Repositories;
using MediatR;

namespace FaqAssistant.Application.Features.Faq.Queries.GetFaqDetailsById;

public class GetFaqDetailsByIdQueryHandler : IRequestHandler<GetFaqDetailsByIdQuery, Result<GetFaqDetailsByIdResponse>>
{
    private readonly IFaqRepository _faqRepository;
    public GetFaqDetailsByIdQueryHandler(IFaqRepository faqRepository)
    {
        _faqRepository = faqRepository ?? throw new ArgumentNullException(nameof(faqRepository));
    }
    public async Task<Result<GetFaqDetailsByIdResponse>> Handle(GetFaqDetailsByIdQuery request, CancellationToken cancellationToken)
    {
        var faqDetails = await _faqRepository.GetFaqDetailsByIdAsync(request.FaqId);
        if (faqDetails == null)
        {
            return new Result<GetFaqDetailsByIdResponse>(false, "FAQ not found.");
        }
        return new Result<GetFaqDetailsByIdResponse>(true, faqDetails);
    }
}
