namespace Mycelium.Api.Devices.SignalR;

public interface IDeviceMessenger
{
    Task SendSecurityScanRequestAsync(int deviceId, CancellationToken cancellationToken = default);
    Task SendRestartRequestAsync(int deviceId, CancellationToken cancellationToken = default);
    Task SendRemoteAccessRequestAsync(int deviceId, CancellationToken cancellationToken = default);
}
