using Microsoft.Extensions.DependencyInjection;
using Mycelium.Api.Devices.SignalR;

namespace Mycelium.Api.Devices;

public static class DependencyInjection
{
    public static IServiceCollection AddDevicesFeature(this IServiceCollection services)
    {
        services.AddSignalR();
        services.AddScoped<IDeviceMessenger, SignalRDeviceMessenger>();

        return services;
    }
}
