using Mediator;
using Mycelium.Api.Auth.Dto;
using Mycelium.Api.Core.Results;

namespace Mycelium.Api.Auth.Login.v1;

public sealed record LoginCommand(string Email, string Password) : ICommand<Result<SignInResponse>>;
