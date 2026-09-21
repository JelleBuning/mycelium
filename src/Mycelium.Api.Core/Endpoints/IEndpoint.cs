using Asp.Versioning;
using Microsoft.AspNetCore.Routing;

namespace Mycelium.Api.Core.Endpoints;

public interface IEndpoint
{
    ApiVersion Version => new(1, 0);

    void MapEndpoint(IEndpointRouteBuilder app);
}
