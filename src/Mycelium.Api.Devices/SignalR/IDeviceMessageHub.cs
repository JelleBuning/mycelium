using Mycelium.Common.SignalR;

namespace Mycelium.Api.Devices.SignalR;

public interface IDeviceMessageHub
{
    Task RestartDeviceMessage(RestartDeviceMessage restartDeviceMessage);
    Task SecurityScanMessage(SecurityScanMessage securityScanMessage);
    Task<RemoteAccessResponseMessage> RemoteAccessMessage(RemoteAccessMessage remoteAccessMessage);
}
