using System.Text.Json;
using Mycelium.Common.DTO.Device;
using Mycelium.WorkerService.Common.Helpers;
using Mycelium.WorkerService.Core.DeviceInformation.Interfaces;

namespace Mycelium.WorkerService.Core.Linux.DeviceInformation;

public class DiskInformationRetriever : IDiskInformationRetriever
{
    public List<DiskDto> Retrieve()
    {
        return DriveInfo.GetDrives()
            .Where(drive => drive.IsReady)
            .Select(drive => new DiskDto
            {
                Name = drive.Name,
                Size = drive.TotalSize,
                Used = drive.TotalSize - drive.TotalFreeSpace,
                IsOsDisk = drive.Name == "/",
                HealthStatus = TryGetHealthStatus(drive.Name)
            })
            .ToList();
    }

    private static string? TryGetHealthStatus(string mountPoint)
    {
        try
        {
            var device = GetDeviceForMountPoint(mountPoint);
            if (device is null)
            {
                return null;
            }

            var parentDisk = GetParentDisk(device);
            var json = RunProcess("smartctl", $"-a -j {parentDisk}");
            return ParseSmartctlHealth(json);
        }
        catch
        {
            return null;
        }
    }

    internal static string? ParseSmartctlHealth(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        if (root.TryGetProperty("smart_status", out var smartStatus) &&
            smartStatus.TryGetProperty("passed", out var passed))
        {
            return passed.GetBoolean() ? "OK" : "Failing";
        }

        return null;
    }

    private static string? GetDeviceForMountPoint(string mountPoint)
    {
        return FindDeviceForMountPoint(File.ReadLines("/proc/mounts"), mountPoint);
    }

    internal static string? FindDeviceForMountPoint(IEnumerable<string> mountLines, string mountPoint)
    {
        foreach (var line in mountLines)
        {
            var parts = line.Split(' ');
            if (parts.Length >= 2 && parts[1] == mountPoint)
            {
                return parts[0];
            }
        }

        return null;
    }

    private static string GetParentDisk(string device)
    {
        var parentName = RunProcess("lsblk", $"-no pkname {device}").Trim();
        return string.IsNullOrEmpty(parentName) ? device : $"/dev/{parentName}";
    }

    private static string RunProcess(string fileName, string arguments)
    {
        var process = ProcessHelper.Start(fileName, arguments);
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        return output;
    }
}
