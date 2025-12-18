using FaqAssistant.Application.Common;
using FaqAssistant.Application.Helpers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace FaqAssistant.Application.Features.User.Commands.AuthorizeUser;

public record AuthorizeUserCommand(string Username, string Password) : IRequest<Result<AuthResponse>>;
