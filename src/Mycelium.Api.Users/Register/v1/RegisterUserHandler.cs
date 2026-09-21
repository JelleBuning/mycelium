using Mediator;
using Microsoft.EntityFrameworkCore;
using OtpNet;
using Mycelium.Api.Core.Results;
using Mycelium.Api.EntityFramework.Entities;
using Mycelium.Api.EntityFramework.Persistence;

namespace Mycelium.Api.Users.Register.v1;

public sealed class RegisterUserHandler(AppDbContext dbContext) : ICommandHandler<RegisterUserCommand, Result>
{
    public async ValueTask<Result> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var emailInUse = await dbContext.Users.AnyAsync(x => x.Email.ToLower() == command.Email.ToLower(), cancellationToken);
        if (emailInUse)
        {
            return Result.Failure(Error.Forbidden("Email already in use"));
        }

        var key = KeyGeneration.GenerateRandomKey(20);
        var twoFactorToken = Base32Encoding.ToString(key);

        var organisation = new Organisation
        {
            Hash = Guid.NewGuid(),
            Users = new List<User>
            {
                new()
                {
                    Email = command.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(command.Password),
                    TwoFactorToken = twoFactorToken,
                }
            }
        };

        await dbContext.Organisations.AddAsync(organisation, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
