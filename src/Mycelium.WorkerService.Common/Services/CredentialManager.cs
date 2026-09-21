using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Mycelium.Common.DTO.Device;
using Mycelium.WorkerService.Common.Services.Interfaces;
using RefreshTokenResponse = Mycelium.WorkerService.Common.DTO.DeviceTokenResponse;

namespace Mycelium.WorkerService.Common.Services;

public class CredentialManager(IConfiguration configuration) : ICredentialManager
{
    private static readonly string Path =
        System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "credentials.json");

    public async Task SetDeviceDetailsAsync(DeviceTokenResponse deviceTokenResponse)
    {
        configuration["Id"] = deviceTokenResponse.Id.ToString();
        configuration["OrganisationId"] = deviceTokenResponse.OrganisationId.ToString();
        configuration["AccessToken"] = deviceTokenResponse.AccessToken;
        configuration["RefreshToken"] = deviceTokenResponse.RefreshToken;
        await File.WriteAllTextAsync(Path, JsonSerializer.Serialize(deviceTokenResponse));
    }

    public async Task SetTokensAsync(RefreshTokenResponse refreshTokenResponse)
    {
        var deviceModel = await GetDeviceDetailsAsync() ?? throw new Exception("No model in storage");
        deviceModel.AccessToken = refreshTokenResponse.AccessToken;
        deviceModel.RefreshToken = refreshTokenResponse.RefreshToken;
        await SetDeviceDetailsAsync(deviceModel);
    }

    public async Task<DeviceTokenResponse?> GetDeviceDetailsAsync()
    {
        if (!File.Exists(Path))
        {
            return null;
        }

        var fileContent = await File.ReadAllTextAsync(Path);
        return JsonSerializer.Deserialize<DeviceTokenResponse>(fileContent);
    }
}