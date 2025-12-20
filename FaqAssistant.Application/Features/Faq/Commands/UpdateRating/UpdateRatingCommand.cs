using FaqAssistant.Application.Helpers;
using MediatR;

namespace FaqAssistant.Application.Features.Faq.Commands.UpdateRating;

public record UpdateRatingCommand(Guid Id, bool IsUpvote) : IRequest<Result<Guid>>;
