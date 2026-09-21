using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mycelium.Common.DTO.Device;
using Mycelium.Common.DTO.Device.Information;
using Mycelium.WorkerService.Common.Api.Extensions;

namespace Mycelium.WorkerService.Common.Api;

public class MyceliumApiService(HttpClient client, IConfiguration configuration, ILogger<MyceliumApiService> logger)
{
    public async Task<DeviceTokenResponse?> RegisterDeviceAsync(Guid organisationHash, string name, CancellationToken cancellationToken)
    {
        try
        {
            var payload = new RegisterDeviceRequest { OrganisationHash = organisationHash, Name = name };
            var output = await client.PostAsync("/api/v1/devices/register", payload, cancellationToken);
            output.EnsureSuccessStatusCode();

            return await output.Content.DeserializeAsync<DeviceTokenResponse>(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to register device. OrganisationHash: {OrganisationHash}, Name: {Name}", organisationHash, name);
            return null;
        }
    }

    public async Task PingAsync()
    {
        var result = await client.PostAsync($"/api/v1/devices/{configuration["Id"]}/ping");
        result.EnsureSuccessStatusCode();
    }

    public async Task UpdateDeviceInformationAsync(DeviceInformationDto deviceInformationDto)
    {
        var result = await client.PutAsync($"/api/v1/devices/{configuration["Id"]}", deviceInformationDto);
        result.EnsureSuccessStatusCode();
    }

    public async Task UpdateStorageInformationAsync(StorageInformationDto storageInformationDto)
    {
        var result = await client.PutAsync($"/api/v1/devices/{configuration["Id"]}/storage", storageInformationDto);
        result.EnsureSuccessStatusCode();
    }

    public async Task UpdateSecurityInformationAsync(SecurityInformationDto securityInformationDto)
    {
        var result = await client.PutAsync($"/api/v1/devices/{configuration["Id"]}/security", securityInformationDto);
        result.EnsureSuccessStatusCode();
    }

    public async Task UpdateSoftwareInformationAsync(SoftwareInformationDto softwareInformationDto)
    {
        var result = await client.PutAsync($"/api/v1/devices/{configuration["Id"]}/software", softwareInformationDto);
        result.EnsureSuccessStatusCode();
    }
}