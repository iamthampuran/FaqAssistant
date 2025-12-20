using FaqAssistant.Application.Helpers;
using FaqAssistant.Application.Interfaces.Common;
using FaqAssistant.Application.Interfaces.Repositories;
using FaqAssistant.Application.Interfaces.Services;
using FaqAssistant.Domain.Entities;
using MediatR;

namespace FaqAssistant.Application.Features.Faq.Commands.CreateFaq;

public class CreateFaqCommandHandler : IRequestHandler<CreateFaqCommand, Result<Guid>>
{
    private readonly IFaqRepository _faqRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateFaqCommandHandler(
        IFaqRepository faqRepository,
        ICategoryRepository categoryRepository,
        ITagRepository tagRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _faqRepository = faqRepository ?? throw new ArgumentNullException(nameof(faqRepository));
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        _tagRepository = tagRepository ?? throw new ArgumentNullException(nameof(tagRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    }

    public async Task<Result<Guid>> Handle(CreateFaqCommand request, CancellationToken cancellationToken)
    {
        var result = new Result<Guid>();

        var currentUserId = _currentUserService.GetCurrentUserId();
        if (currentUserId == null)
        {
            return new Result<Guid>(false, "User is not authenticated.");
        }

        if (currentUserId != request.UserId)
        {
            return new Result<Guid>(false, "You are not authorized to add details for other user.");
        }

        // Validate Category exists
        var categoryExists = await _categoryRepository.ExistsAsync(request.CategoryId, cancellationToken);
        if (!categoryExists)
        {
            result.Message = "Category not found.";
            return result;
        }

        // Validate all Tags exist
        if (request.TagIds.Count != 0)
        {
            var tags = await _tagRepository.GetAsync(t => request.TagIds.Contains(t.Id) && !t.IsDeleted);
            if (tags.Count != request.TagIds.Count)
            {
                result.Message = "One or more tags not found.";
                return result;
            }
        }

        // Create FAQ
        var newFaq = new Domain.Entities.Faq
        {
            Id = Guid.NewGuid(),
            Question = request.Question,
            Answer = request.Answer,
            UserId = request.UserId,
            CategoryId = request.CategoryId,
            CreatedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        var ratings = new Domain.Entities.Rating
        {
            Id = Guid.NewGuid(),
            FaqId = newFaq.Id,
            UserId = request.UserId,
            IsUpvote = true,
            CreatedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        // Create FaqTag relationships
        var faqTags = request.TagIds.Select(tagId => new FaqTag
        {
            Id = Guid.NewGuid(),
            FaqId = newFaq.Id,
            TagId = tagId,
            CreatedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        }).ToList();

        newFaq.Tags = faqTags;

        await _faqRepository.AddAsync(newFaq, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        result.Success = true;
        result.Data = newFaq.Id;
        return result;
    }
}