using FaqAssistant.Application.Helpers;
using FaqAssistant.Application.Interfaces.Common;
using FaqAssistant.Application.Interfaces.Repositories;
using MediatR;

namespace FaqAssistant.Application.Features.Tag.Commands.DeleteTag;

public class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand, Result<Guid>>
{
    private readonly ITagRepository _tagRepository;
    private readonly IUnitOfWork _unitOfWork;
    public DeleteTagCommandHandler(ITagRepository tagRepository, IUnitOfWork unitOfWork)
    {
        _tagRepository = tagRepository ?? throw new ArgumentNullException(nameof(tagRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }
    public async Task<Result<Guid>> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var result = new Result<Guid>();
        var existingTag = await _tagRepository.GetByIdAsync(request.Id, cancellationToken);
        if (existingTag == null || existingTag.IsDeleted)
        {
            result.Message = "Tag not found.";
            return result;
        }
        existingTag.IsDeleted = true;
        existingTag.LastUpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        result.Success = true;
        result.Data = existingTag.Id;
        return result;
    }
}
