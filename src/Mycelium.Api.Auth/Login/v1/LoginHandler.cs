using System.Security.Claims;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Mycelium.Api.Auth.Dto;
using Mycelium.Api.Core.Results;
using Mycelium.Api.Core.Security;
using Mycelium.Api.EntityFramework.Persistence;

namespace Mycelium.Api.Auth.Login.v1;

public sealed class LoginHandler(AppDbContext dbContext, ITokenGenerator tokenGenerator)
    : ICommandHandler<LoginCommand, Result<SignInResponse>>
{
    public async ValueTask<Result<SignInResponse>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(x => x.Email.ToLower() == command.Email.ToLower(), cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(command.Password, user.Password))
        {
            return Result.Failure<SignInResponse>(Error.Unauthorized("Invalid login credentials"));
        }

        var claims = new List<Claim> { new(ClaimTypes.Email, user.Email) };
        user.AuthenticityToken = tokenGenerator.GenerateAccessToken(claims);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new SignInResponse
        {
            UserId = user.Id,
            OrganisationId = user.OrganisationId,
            AuthenticityToken = user.AuthenticityToken,
            TwoFactorToken = user.LastVerified is null ? user.TwoFactorToken : null
        });
    }
}
