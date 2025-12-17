using FaqAssistant.Application.Helpers;
using MediatR;

namespace FaqAssistant.Application.Features.User.Commands.UpdateUser;

public class UpdateUserCommand : IRequest<Result<Guid>>
{
    public Guid Id { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
}
