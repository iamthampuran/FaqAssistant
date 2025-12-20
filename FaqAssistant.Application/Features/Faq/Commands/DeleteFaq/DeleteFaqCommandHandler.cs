using FaqAssistant.Application.Helpers;
using FaqAssistant.Application.Interfaces.Common;
using FaqAssistant.Application.Interfaces.Repositories;
using MediatR;

namespace FaqAssistant.Application.Features.Faq.Commands.DeleteFaq;

public class DeleteFaqCommandHandler : IRequestHandler<DeleteFaqCommand, Result<Guid>>
{
    private readonly IFaqRepository _faqRepository;
    private readonly IUnitOfWork _unitOfWork;
    public DeleteFaqCommandHandler(IFaqRepository faqRepository, IUnitOfWork unitOfWork)
    {
        _faqRepository = faqRepository ?? throw new ArgumentNullException(nameof(faqRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }
    public async Task<Result<Guid>> Handle(DeleteFaqCommand request, CancellationToken cancellationToken)
    {
        var result = new Result<Guid>();
        var faq = await _faqRepository.GetFirstAsync(
            faq => faq.Id == request.Id,
            disableTracking: false,
            includes: [faq => faq.Tags],
            cancellationToken: cancellationToken);
        if (faq == null || faq.IsDeleted)
        {
            result.Message = "Faq not found.";
            return result;
        }
        faq.IsDeleted = true;
        
        foreach (var tag in faq.Tags)
        {
            tag.IsDeleted = true;
        }
        
        faq.LastUpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        result.Success = true;
        result.Data = faq.Id;
        result.Message = "Faq deleted successfully.";
        return result;
    }
}
