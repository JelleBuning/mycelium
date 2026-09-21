using Mediator;
using Mycelium.Api.Auth.Dto;
using Mycelium.Api.Core.Results;

namespace Mycelium.Api.Auth.VerifyTotp.v1;

public sealed record VerifyTotpCommand(int UserId, string AuthenticityToken, string OtpAttempt) : ICommand<Result<TokenDto>>;
