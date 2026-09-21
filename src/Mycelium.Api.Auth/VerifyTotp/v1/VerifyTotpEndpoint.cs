using Microsoft.AspNetCore.Builder;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Mycelium.Api.Core.Endpoints;
using Mycelium.Api.Core.Results;

namespace Mycelium.Api.Auth.VerifyTotp.v1;

public sealed class VerifyTotpEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/users/verify", async ([FromBody] VerifyTotpCommand command, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(command, cancellationToken);
                return result.ToHttpResult();
            })
            .WithName("VerifyTotp")
            .WithTags("Auth");
    }
}
