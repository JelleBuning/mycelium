using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;

namespace Mycelium.Api.Devices.SignalR;

public static class DeviceHubMappingExtensions
{
    public static IEndpointRouteBuilder MapDeviceMessageHub(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHub<DeviceMessageHub>(nameof(DeviceMessageHub));
        return endpoints;
    }
}
