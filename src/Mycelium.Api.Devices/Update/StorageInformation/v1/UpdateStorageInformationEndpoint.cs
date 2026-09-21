using Microsoft.AspNetCore.Builder;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Mycelium.Api.Core.Endpoints;
using Mycelium.Api.Core.Results;
using Mycelium.Common.DTO.Device;

namespace Mycelium.Api.Devices.Update.StorageInformation.v1;

public sealed class UpdateStorageInformationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/devices/{id}/storage", async ([FromRoute] int id, [FromBody] StorageInformationDto body, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new UpdateStorageInformationCommand(id, body), cancellationToken);
                return result.ToHttpResult();
            })
            .WithName("UpdateStorageInformation")
            .WithTags("Devices")
            .RequireAuthorization("Device");
    }
}
