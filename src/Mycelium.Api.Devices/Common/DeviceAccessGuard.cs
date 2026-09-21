using System.Security.Claims;
using Mycelium.Api.Core.Results;

namespace Mycelium.Api.Devices.Common;

/// <summary>
/// A device's JWT only lets it act on itself — this checks the "Id" claim against
/// the target device id, shared by every device-self mutating endpoint (ping, updates).
/// </summary>
public static class DeviceAccessGuard
{
    public static Error? Validate(ClaimsPrincipal? user, int deviceId)
    {
        if (user is null)
        {
            return Error.Unauthorized("Unauthorized");
        }

        var claim = user.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        if (!int.TryParse(claim, out var tokenDeviceId) || tokenDeviceId != deviceId)
        {
            return Error.Forbidden("No access to other devices");
        }

        return null;
    }
}
