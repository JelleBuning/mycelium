using FluentValidation;

namespace Mycelium.Api.Auth.RefreshToken.v1;

public sealed class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty().WithMessage("AccessToken is required");

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("RefreshToken is required");
    }
}
