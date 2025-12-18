using FaqAssistant.Application.Helpers;
using FaqAssistant.Application.Interfaces.Common;
using FaqAssistant.Application.Interfaces.Repositories;
using MediatR;

namespace FaqAssistant.Application.Features.Tag.Commands.UpdateTag
{
    public class UpdateTagCommandHandler : IRequestHandler<UpdateTagCommand, Result<Guid>>
    {
        private readonly ITagRepository _tagRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTagCommandHandler(ITagRepository tagRepository, IUnitOfWork unitOfWork)
        {
            _tagRepository = tagRepository ?? throw new ArgumentNullException(nameof(tagRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Result<Guid>> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
        {
            var existingTag = await _tagRepository.GetFirstAsync(t => t.Id == request.Id || t.Name == request.Name, cancellationToken);
            var result = new Result<Guid>();
            if (existingTag == null || existingTag.IsDeleted)
            {
                result.Message = "Tag not found.";
                return result;
            }
            else if (existingTag.Id != request.Id)
            {
                result.Message = $"Another tag with the name '{request.Name}' already exists.";
                return result;
            }
            existingTag.Name = request.Name;
            existingTag.LastUpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            result.Success = true;
            result.Data = existingTag.Id;
            return result;
        }
    }
}
