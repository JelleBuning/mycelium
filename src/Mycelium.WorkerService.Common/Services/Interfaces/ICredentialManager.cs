using Mycelium.Common.DTO.Device;
using RefreshTokenResponse = Mycelium.WorkerService.Common.DTO.DeviceTokenResponse;

namespace Mycelium.WorkerService.Common.Services.Interfaces;

public interface ICredentialManager
{
    public Task SetDeviceDetailsAsync(DeviceTokenResponse deviceTokenResponse);
    public Task SetTokensAsync(RefreshTokenResponse refreshTokenResponse);
    public Task<DeviceTokenResponse?> GetDeviceDetailsAsync();
}