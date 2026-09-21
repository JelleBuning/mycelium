using System.Security.Claims;

namespace Mycelium.Api.Core.Security;

public interface ITokenGenerator
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
