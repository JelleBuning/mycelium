using Microsoft.AspNetCore.Builder;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Mycelium.Api.Core.Endpoints;
using Mycelium.Api.Core.Results;

namespace Mycelium.Api.Devices.GetStorageInformation.v1;

public sealed class GetStorageInformationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/devices/{id}/storage", async ([FromRoute] int id, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new GetStorageInformationQuery(id), cancellationToken);
                return result.ToHttpResult();
            })
            .WithName("GetStorageInformation")
            .WithTags("Devices")
            .RequireAuthorization("User");
    }
}
