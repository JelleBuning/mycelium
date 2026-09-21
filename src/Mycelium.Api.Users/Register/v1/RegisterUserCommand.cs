using Mediator;
using Mycelium.Api.Core.Results;

namespace Mycelium.Api.Users.Register.v1;

public sealed record RegisterUserCommand(string Email, string Password) : ICommand<Result>;
