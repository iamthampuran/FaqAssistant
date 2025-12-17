using FaqAssistant.Application.Helpers;
using MediatR;

namespace FaqAssistant.Application.Features.User.Commands.CreateUser;

public class CreateUserCommand : IRequest<Result<Guid>>
{
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
