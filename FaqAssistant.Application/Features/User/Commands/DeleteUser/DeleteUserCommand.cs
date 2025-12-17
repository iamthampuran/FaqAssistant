using FaqAssistant.Application.Helpers;
using MediatR;

namespace FaqAssistant.Application.Features.User.Commands.DeleteUser;

public record DeleteUserCommand(Guid UserId) : IRequest<Result<Guid>>;
