using Microsoft.AspNetCore.Builder;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Mycelium.Api.Core.Endpoints;
using Mycelium.Api.Core.Results;

namespace Mycelium.Api.Users.Register.v1;

public sealed class RegisterUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/register", async ([FromBody] RegisterUserCommand command, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(command, cancellationToken);
                return result.ToHttpResult();
            })
            .WithName("RegisterUser")
            .WithTags("Users");
    }
}
