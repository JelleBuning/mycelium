using System.Security.Claims;
using Mediator;
using Microsoft.EntityFrameworkCore;
using OtpNet;
using Mycelium.Api.Auth.Dto;
using Mycelium.Api.Core.Results;
using Mycelium.Api.Core.Security;
using Mycelium.Api.EntityFramework.Persistence;

namespace Mycelium.Api.Auth.VerifyTotp.v1;

public sealed class VerifyTotpHandler(AppDbContext dbContext, ITokenGenerator tokenGenerator)
    : ICommandHandler<VerifyTotpCommand, Result<TokenDto>>
{
    public async ValueTask<Result<TokenDto>> Handle(VerifyTotpCommand command, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == command.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<TokenDto>(Error.Validation("Invalid user id"));
        }

        if (user.AuthenticityToken != command.AuthenticityToken)
        {
            return Result.Failure<TokenDto>(Error.Unauthorized("Invalid authenticity token"));
        }

        var totp = new Totp(Base32Encoding.ToBytes(user.TwoFactorToken), step: 30, mode: OtpHashMode.Sha1, totpSize: 6);
        var valid = totp.VerifyTotp(command.OtpAttempt, out _, window: VerificationWindow.RfcSpecifiedNetworkDelay);
        if (!valid)
        {
            return Result.Failure<TokenDto>(Error.Unauthorized("Invalid authenticator code"));
        }

        user.LastVerified = DateTime.Now;

        var claims = new List<Claim>
        {
            new("Id", user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Type.ToString()),
        };

        user.RefreshToken = tokenGenerator.GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMonths(1);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new TokenDto
        {
            AccessToken = tokenGenerator.GenerateAccessToken(claims),
            RefreshToken = user.RefreshToken
        });
    }
}
