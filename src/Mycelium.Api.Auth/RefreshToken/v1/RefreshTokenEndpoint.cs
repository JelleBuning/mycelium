using Microsoft.AspNetCore.Builder;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Mycelium.Api.Core.Endpoints;
using Mycelium.Api.Core.Results;

namespace Mycelium.Api.Auth.RefreshToken.v1;

public sealed class RefreshTokenEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/refresh", async ([FromBody] RefreshTokenCommand command, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(command, cancellationToken);
                return result.ToHttpResult();
            })
            .WithName("RefreshToken")
            .WithTags("Auth")
            .AllowAnonymous();
    }
}
