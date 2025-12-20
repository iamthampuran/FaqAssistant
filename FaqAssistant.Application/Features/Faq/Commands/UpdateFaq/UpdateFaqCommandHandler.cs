using FaqAssistant.Application.Helpers;
using FaqAssistant.Application.Interfaces.Common;
using FaqAssistant.Application.Interfaces.Repositories;
using FaqAssistant.Domain.Entities;
using MediatR;

namespace FaqAssistant.Application.Features.Faq.Commands.UpdateFaq;

public class UpdateFaqCommandHandler : IRequestHandler<UpdateFaqCommand, Result<Guid>>
{
    private readonly IFaqRepository _faqRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public UpdateFaqCommandHandler(IFaqRepository faqRepository, IUnitOfWork unitOfWork)
    {
        _faqRepository = faqRepository ?? throw new ArgumentNullException(nameof(faqRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<Guid>> Handle(UpdateFaqCommand command, CancellationToken cancellationToken)
    {
        var faq = await _faqRepository.GetFirstAsync(
            faq => faq.Id == command.Id, 
            [faq => faq.Tags], 
            disableTracking: false, 
            cancellationToken: cancellationToken);

        if (faq == null || faq.IsDeleted)
        {
            return new Result<Guid>(false, "Faq was not found");
        }


        // Update basic properties
        faq.Question = command.Question;
        faq.Answer = command.Answer;
        faq.CategoryId = command.CategoryId;
        faq.UserId = command.UserId;
        faq.LastUpdatedAt = DateTime.UtcNow;

        var incomingTagIds = (command.TagIds ?? []).ToHashSet();
        var now = DateTime.UtcNow;

        // CASE 1: No tags sent → soft delete ALL faq tags
        if (incomingTagIds.Count == 0)
        {
            foreach (var faqTag in faq.Tags.Where(t => !t.IsDeleted))
            {
                faqTag.IsDeleted = true;
                faqTag.LastUpdatedAt = now;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new Result<Guid>(true, faq.Id, "Faq updated successfully");
        }

        // CASE 2: Soft delete removed tags
        foreach (var faqTag in faq.Tags
            .Where(t => !t.IsDeleted && !incomingTagIds.Contains(t.TagId)))
        {
            faqTag.IsDeleted = true;
            faqTag.LastUpdatedAt = now;
        }

        // CASE 3: Reactivate previously deleted tags
        foreach (var faqTag in faq.Tags
            .Where(t => t.IsDeleted && incomingTagIds.Contains(t.TagId)))
        {
            faqTag.IsDeleted = false;
            faqTag.LastUpdatedAt = now;
        }

        // CASE 4: Add brand-new tags
        var existingTagIds = faq.Tags.Select(t => t.TagId).ToHashSet();

        await _faqRepository.AddNewTags([.. incomingTagIds.Except(existingTagIds)], faq.Id);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new Result<Guid>(true, faq.Id, "Faq updated successfully");
    }
}
