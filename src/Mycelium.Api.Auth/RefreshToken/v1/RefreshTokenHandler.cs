using System.Security.Claims;
using Mediator;
using Mycelium.Api.Auth.Dto;
using Mycelium.Api.Core.Results;
using Mycelium.Api.Core.Security;
using Mycelium.Api.EntityFramework.Persistence;

namespace Mycelium.Api.Auth.RefreshToken.v1;

public sealed class RefreshTokenHandler(AppDbContext dbContext, ITokenGenerator tokenGenerator)
    : ICommandHandler<RefreshTokenCommand, Result<TokenDto>>
{
    public async ValueTask<Result<TokenDto>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var principal = tokenGenerator.GetPrincipalFromExpiredToken(command.AccessToken);
        var claimId = principal.Claims.Single(c => c.Type == "Id").Value;
        var role = principal.Claims.Single(c => c.Type == ClaimTypes.Role).Value;

        switch (role)
        {
            case "Admin":
            case "User":
                var user = dbContext.Users.SingleOrDefault(x => x.Id.ToString() == claimId);
                if (user is null || user.RefreshToken != command.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                {
                    return Result.Failure<TokenDto>(Error.Validation("Invalid refresh token"));
                }

                user.RefreshToken = tokenGenerator.GenerateRefreshToken();
                await dbContext.SaveChangesAsync(cancellationToken);
                return Result.Success(new TokenDto
                {
                    AccessToken = tokenGenerator.GenerateAccessToken(principal.Claims),
                    RefreshToken = user.RefreshToken
                });

            case "Device":
                var device = dbContext.Devices.SingleOrDefault(x => x.Id.ToString() == claimId);
                if (device is null || device.RefreshToken != command.RefreshToken)
                {
                    return Result.Failure<TokenDto>(Error.Validation("Invalid refresh token"));
                }

                device.RefreshToken = tokenGenerator.GenerateRefreshToken();
                await dbContext.SaveChangesAsync(cancellationToken);
                return Result.Success(new TokenDto
                {
                    AccessToken = tokenGenerator.GenerateAccessToken(principal.Claims),
                    RefreshToken = device.RefreshToken
                });

            default:
                throw new InvalidOperationException($"Invalid role '{role}' in refresh token claims.");
        }
    }
}
