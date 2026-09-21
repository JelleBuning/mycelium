using Mycelium.Common.DTO.Device;

namespace Mycelium.WorkerService.Common.Api.Interfaces;

public interface IAuthenticationHandler
{
    public Task<DeviceTokenResponse> EnsureAuthenticatedAsync(Guid organisationHash, string name, CancellationToken cancellationToken);
}