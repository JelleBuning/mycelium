using Microsoft.AspNetCore.Builder;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Mycelium.Api.Core.Endpoints;
using Mycelium.Api.Core.Results;

namespace Mycelium.Api.Organisations.GetAll.v1;

public sealed class GetAllOrganisationsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/organisations", async ([FromServices] IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new GetAllOrganisationsQuery(), cancellationToken);
                return result.ToHttpResult();
            })
            .WithName("GetAllOrganisations")
            .WithTags("Organisations")
            .RequireAuthorization("User");
    }
}
