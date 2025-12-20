using FaqAssistant.Application.Helpers;
using FaqAssistant.Application.Interfaces.Common;
using FaqAssistant.Application.Interfaces.Repositories;
using FaqAssistant.Application.Interfaces.Services;
using FaqAssistant.Domain.Entities;
using MediatR;

namespace FaqAssistant.Application.Features.Faq.Commands.UpdateRating;

public class UpdateRatingCommandHandler : IRequestHandler<UpdateRatingCommand, Result<Guid>>
{
    private readonly IFaqRepository _faqRepository;
    private readonly IGenericRepository<Rating> _ratingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdateRatingCommandHandler(
        IFaqRepository faqRepository,
        IGenericRepository<Rating> ratingRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _faqRepository = faqRepository ?? throw new ArgumentNullException(nameof(faqRepository));
        _ratingRepository = ratingRepository ?? throw new ArgumentNullException(nameof(ratingRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    }

    public async Task<Result<Guid>> Handle(UpdateRatingCommand request, CancellationToken cancellationToken)
    {
        var result = new Result<Guid>();

        var currentUserId = _currentUserService.GetCurrentUserId();
        if (currentUserId == null)
        {
            result.Message = "User is not authenticated.";
            return result;
        }

        var faq = await _faqRepository.GetFirstAsync(
            faq => faq.Id == request.Id,
            disableTracking: false,
            includes: [faq => faq.Ratings],
            cancellationToken: cancellationToken);
        
        if (faq == null || faq.IsDeleted)
        {
            result.Message = "Faq not found.";
            return result;
        }

        var currentUserRating = faq.Ratings.FirstOrDefault(r => r.UserId == currentUserId && !r.IsDeleted);
        if (currentUserRating != null && currentUserRating.IsUpvote == request.IsUpvote)
        {
            result.Message = "You have already submitted this rating.";
            return result;
        }
        else if (currentUserRating != null && currentUserRating.IsUpvote != request.IsUpvote)
        {
            currentUserRating.IsUpvote = request.IsUpvote;
            currentUserRating.LastUpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            result.Success = true;
            result.Data = faq.Id;
            return result;
        }

        var newRating = new Rating
        {
            Id = Guid.NewGuid(),
            FaqId = request.Id,
            UserId = currentUserId.Value,
            IsUpvote = request.IsUpvote,
            CreatedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _ratingRepository.AddAsync(newRating, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        result.Success = true;
        result.Data = faq.Id;
        return result;
    }
}
