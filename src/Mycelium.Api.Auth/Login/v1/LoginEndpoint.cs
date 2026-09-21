using Microsoft.AspNetCore.Builder;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Mycelium.Api.Core.Endpoints;
using Mycelium.Api.Core.Results;

namespace Mycelium.Api.Auth.Login.v1;

public sealed class LoginEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/users/sign_in", async ([FromBody] LoginCommand command, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(command, cancellationToken);
                return result.ToHttpResult();
            })
            .WithName("Login")
            .WithTags("Auth");
    }
}
