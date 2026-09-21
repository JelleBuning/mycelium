using Mediator;
using Mycelium.Api.Auth.Dto;
using Mycelium.Api.Core.Results;

namespace Mycelium.Api.Auth.RefreshToken.v1;

public sealed record RefreshTokenCommand(string AccessToken, string RefreshToken) : ICommand<Result<TokenDto>>;
